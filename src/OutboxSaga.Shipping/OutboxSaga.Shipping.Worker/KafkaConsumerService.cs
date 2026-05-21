using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OutboxSaga.Shipping.Worker.Contracts;
using OutboxSaga.Shipping.Worker.Handlers;

namespace OutboxSaga.Shipping.Worker;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly ConsumerConfig _consumerConfig;
    private readonly OrderPaidHandler _handler;
    private const string OrderPaidTopic = "order-paid";

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<ConsumerConfig> consumerConfig,
        OrderPaidHandler handler)
    {
        _logger = logger;
        _consumerConfig = consumerConfig.Value;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
        consumer.Subscribe(OrderPaidTopic);

        _logger.LogInformation("Kafka Consumer started for topic {Topic}", OrderPaidTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result == null) continue;

                var @event = JsonSerializer.Deserialize<OrderPaidIntegrationEvent>(result.Message.Value, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (@event != null)
                {
                    string? correlationId = null;
                    if (result.Message.Headers.TryGetLastBytes("CorrelationId", out var correlationIdBytes))
                    {
                        correlationId = Encoding.UTF8.GetString(correlationIdBytes);
                    }

                    string? causationId = null;
                    if (result.Message.Headers.TryGetLastBytes("CausationId", out var causationIdBytes))
                    {
                        causationId = Encoding.UTF8.GetString(causationIdBytes);
                    }

                    await _handler.HandleAsync(@event, correlationId, causationId, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message");
            }
        }
    }
}
