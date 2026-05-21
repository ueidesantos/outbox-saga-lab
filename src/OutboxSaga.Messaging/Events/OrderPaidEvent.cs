namespace OutboxSaga.Messaging.Events;
public record OrderPaidEvent(Guid OrderId, DateTime PaidAtUtc);
