namespace OutboxSaga.Shipping.Worker.Contracts;

public sealed record OrderPaidIntegrationEvent(
    Guid OrderId,
    DateTime PaidAtUtc);
