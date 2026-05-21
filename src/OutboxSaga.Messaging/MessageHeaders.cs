namespace OutboxSaga.Messaging;

public static class MessageHeaders
{
    public const string MessageId = "message_id";
    public const string EventType = "event_type";
    public const string EventVersion = "event_version";
    public const string Source = "source";
    public const string CorrelationId = "correlation_id";
    public const string CausationId = "causation_id";
    public const string OccurredOnUtc = "occurred_on_utc";
}
