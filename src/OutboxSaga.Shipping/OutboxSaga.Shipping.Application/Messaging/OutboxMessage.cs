using System.Text.Json;
using OutboxSaga.Shipping.Domain.Common;

namespace OutboxSaga.Shipping.Application.Messaging;

public sealed class OutboxMessage
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string AggregateId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public string? CausationId { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; private set; }

    public static OutboxMessage FromDomainEvent(
        string aggregateId,
        IDomainEvent domainEvent,
        string? correlationId = null,
        string? causationId = null)
    {
        return new OutboxMessage
        {
            AggregateId = aggregateId,
            EventType = domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), SerializerOptions),
            OccurredOnUtc = domainEvent.OccurredOnUtc,
            CorrelationId = correlationId,
            CausationId = causationId
        };
    }

    public void MarkAsPublished(DateTime publishedAtUtc)
    {
        PublishedAtUtc = publishedAtUtc;
    }
}
