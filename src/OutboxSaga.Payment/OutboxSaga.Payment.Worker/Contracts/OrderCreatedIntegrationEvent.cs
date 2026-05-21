namespace OutboxSaga.Payment.Worker.Contracts;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string CustomerId,
    string CustomerName,
    string CustomerEmail,
    decimal TotalValue,
    string Description,
    DateTime CreatedAtUtc);
