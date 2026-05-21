using OutboxSaga.Shipping.Domain.Common;

namespace OutboxSaga.Shipping.Domain.Events;

public record ShippingArrangedDomainEvent(
    Guid ShippingId,
    Guid OrderId,
    string TrackingCode,
    DateTime OccurredOnUtc) : IDomainEvent;
