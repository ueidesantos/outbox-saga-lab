namespace OutboxSaga.Orders.Infrastructure.Outbox.Contracts;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string CustomerId,
    string CustomerName,
    string CustomerEmail,
    decimal TotalValue,
    string Description,
    DateTime CreatedAtUtc);
