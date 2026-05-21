namespace OutboxSaga.Messaging.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    decimal TotalValue,
    string Description,
    DateTime CreatedAtUtc);
