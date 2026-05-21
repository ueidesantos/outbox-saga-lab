using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OutboxSaga.Payment.Application.Abstractions.Messaging;
using OutboxSaga.Payment.Domain.Events;
using OutboxSaga.Payment.Worker.Contracts;
using Polly;
using Polly.Retry;

namespace OutboxSaga.Payment.Worker.Publishers;

public sealed class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly ProducerConfig _producerConfig;
    private readonly AsyncRetryPolicy _retryPolicy;
    private const string PaymentProcessedTopic = "payment-processed";

    public OutboxPublisherService(
        IServiceProvider serviceProvider,
        ILogger<OutboxPublisherService> logger,
        IOptions<ProducerConfig> producerConfig)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _producerConfig = producerConfig.Value;

        _retryPolicy = Policy
            .Handle<KafkaException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var producerConfig = new ProducerConfig(_producerConfig)
        {
            Acks = Acks.All,
            EnableIdempotence = true
        };

        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishMessagesAsync(producer, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxPublisher");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishMessagesAsync(IProducer<string, string> producer, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        var messages = await outboxRepository.GetUnpublishedAsync(batchSize: 20, ct: ct);

        foreach (var message in messages)
        {
            object? eventToPublish = MapToExternalEvent(message);

            if (eventToPublish is not null)
            {
                var payload = JsonSerializer.Serialize(eventToPublish);

                var kafkaMessage = new Message<string, string>
                {
                    Key = message.AggregateId,
                    Value = payload,
                    Headers = new Headers
                    {
                        { "CorrelationId", System.Text.Encoding.UTF8.GetBytes(message.CorrelationId ?? "") },
                        { "CausationId", System.Text.Encoding.UTF8.GetBytes(message.Id) }
                    }
                };

                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await producer.ProduceAsync(PaymentProcessedTopic, kafkaMessage, ct);
                });
            }

            await outboxRepository.MarkAsPublishedAsync(message.Id, DateTime.UtcNow, ct);
        }
    }

    private static object? MapToExternalEvent(Application.Messaging.OutboxMessage message)
    {
        if (message.EventType.Contains(nameof(PaymentAuthorizedDomainEvent)))
        {
            var domainEvent = JsonSerializer.Deserialize<PaymentAuthorizedDomainEvent>(message.Payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (domainEvent != null)
            {
                return new PaymentProcessedIntegrationEvent(
                    domainEvent.PaymentId,
                    domainEvent.OrderId,
                    true,
                    Guid.NewGuid().ToString(),
                    null,
                    domainEvent.OccurredOnUtc);
            }
        }

        return null;
    }
}
