using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OutboxSaga.Messaging.Events;
using OutboxSaga.Payment.Application.Abstractions.Messaging;
using OutboxSaga.Payment.Application.Abstractions.Persistence;
using OutboxSaga.Payment.Application.Messaging;

namespace OutboxSaga.Payment.Worker.Handlers;

public class OrderCreatedHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(IServiceProvider serviceProvider, ILogger<OrderCreatedHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, string? correlationId, string? causationId, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _logger.LogInformation("Processing OrderCreated for Order {OrderId}. CorrelationId: {CorrelationId}", @event.OrderId, correlationId);

        if (await paymentRepository.ExistsForOrderAsync(@event.OrderId, ct))
        {
            _logger.LogWarning("Payment for Order {OrderId} already processed. Skipping.", @event.OrderId);
            return;
        }

        var payment = new Domain.Aggregates.PaymentAggregate.Payment(@event.OrderId, @event.TotalValue);

        await unitOfWork.ExecuteInTransactionAsync(async transactionCt =>
        {
            await paymentRepository.AddAsync(payment, transactionCt);

            foreach (var domainEvent in payment.DomainEvents)
            {
                var outboxMessage = OutboxMessage.FromDomainEvent(
                    aggregateId: payment.Id.ToString(),
                    domainEvent: domainEvent,
                    correlationId: correlationId,
                    causationId: causationId);

                await outboxRepository.AddAsync(outboxMessage, transactionCt);
            }

            payment.ClearDomainEvents();
        }, ct);

        _logger.LogInformation("Payment {PaymentId} authorized for Order {OrderId}", payment.Id, @event.OrderId);
    }
}
