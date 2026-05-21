using OutboxSaga.Payment.Domain.Common;

namespace OutboxSaga.Payment.Domain.Events;

public record PaymentAuthorizedDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime OccurredOnUtc) : IDomainEvent;
