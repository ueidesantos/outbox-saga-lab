using OutboxSaga.Shipping.Domain.Common;
using OutboxSaga.Shipping.Domain.Events;

namespace OutboxSaga.Shipping.Domain.Aggregates.ShippingAggregate;

public class Shipping : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string TrackingCode { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public Shipping(Guid orderId)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        TrackingCode = $"TRK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        Status = "Arranged";
        CreatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new ShippingArrangedDomainEvent(Id, OrderId, TrackingCode, CreatedAtUtc));
    }

    private Shipping(Guid id, Guid orderId, string trackingCode, string status, DateTime createdAtUtc)
    {
        Id = id;
        OrderId = orderId;
        TrackingCode = trackingCode;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public static Shipping Rehydrate(Guid id, Guid orderId, string trackingCode, string status, DateTime createdAtUtc)
    {
        return new Shipping(id, orderId, trackingCode, status, createdAtUtc);
    }
}
