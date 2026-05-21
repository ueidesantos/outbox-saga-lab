namespace OutboxSaga.Shipping.Worker.Contracts;

public sealed record ShippingArrangedIntegrationEvent(
    Guid ShippingId,
    Guid OrderId,
    string TrackingCode,
    DateTime ArrangedAtUtc);
