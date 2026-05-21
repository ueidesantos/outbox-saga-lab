namespace OutboxSaga.Payment.Worker.Contracts;

public sealed record PaymentProcessedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    bool Success,
    string? TransactionId,
    string? ErrorMessage,
    DateTime ProcessedAtUtc);
