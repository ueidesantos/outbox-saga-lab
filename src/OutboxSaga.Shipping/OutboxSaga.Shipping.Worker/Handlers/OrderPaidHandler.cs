using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OutboxSaga.Messaging.Events;
using OutboxSaga.Shipping.Application.Abstractions.Messaging;
using OutboxSaga.Shipping.Application.Abstractions.Persistence;
using OutboxSaga.Shipping.Application.Messaging;

namespace OutboxSaga.Shipping.Worker.Handlers;

public class OrderPaidHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderPaidHandler> _logger;

    public OrderPaidHandler(IServiceProvider serviceProvider, ILogger<OrderPaidHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleAsync(OrderPaidEvent @event, string? correlationId, string? causationId, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var shippingRepository = scope.ServiceProvider.GetRequiredService<IShippingRepository>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _logger.LogInformation("Processing OrderPaid for Order {OrderId}. CorrelationId: {CorrelationId}", @event.OrderId, correlationId);

        if (await shippingRepository.ExistsForOrderAsync(@event.OrderId, ct))
        {
            _logger.LogWarning("Shipping for Order {OrderId} already processed. Skipping.", @event.OrderId);
            return;
        }

        var shipping = new Domain.Aggregates.ShippingAggregate.Shipping(@event.OrderId);

        await unitOfWork.ExecuteInTransactionAsync(async transactionCt =>
        {
            await shippingRepository.AddAsync(shipping, transactionCt);

            foreach (var domainEvent in shipping.DomainEvents)
            {
                var outboxMessage = OutboxMessage.FromDomainEvent(
                    aggregateId: shipping.Id.ToString(),
                    domainEvent: domainEvent,
                    correlationId: correlationId,
                    causationId: causationId);

                await outboxRepository.AddAsync(outboxMessage, transactionCt);
            }

            shipping.ClearDomainEvents();
        }, ct);

        _logger.LogInformation("Shipping {ShippingId} arranged for Order {OrderId}. Tracking: {TrackingCode}", shipping.Id, @event.OrderId, shipping.TrackingCode);
    }
}
