namespace OutboxSaga.Messaging.Events;

public record PaymentProcessedEvent(
    Guid PaymentId,
    Guid OrderId,
    bool Success,
    string? TransactionId,
    string? ErrorMessage,
    DateTime ProcessedAtUtc);
