namespace OutboxSaga.Messaging.Events;

public record ShippingArrangedEvent(
    Guid ShippingId,
    Guid OrderId,
    string TrackingCode,
    DateTime ArrangedAtUtc);
