namespace OutboxSaga.Messaging;

public sealed record IntegrationEventEnvelope<TPayload>(
    string MessageId,
    string EventType,
    int EventVersion,
    string Source,
    string CorrelationId,
    string? CausationId,
    DateTime OccurredOnUtc,
    TPayload Payload);
