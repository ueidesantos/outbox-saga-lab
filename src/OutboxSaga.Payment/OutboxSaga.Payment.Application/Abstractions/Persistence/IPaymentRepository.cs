using OutboxSaga.Payment.Domain.Aggregates.PaymentAggregate;

namespace OutboxSaga.Payment.Application.Abstractions.Persistence;

public interface IPaymentRepository
{
    Task AddAsync(Domain.Aggregates.PaymentAggregate.Payment payment, CancellationToken ct = default);
    Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken ct = default);
}
