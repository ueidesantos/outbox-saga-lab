using OutboxSaga.Shipping.Domain.Aggregates.ShippingAggregate;

namespace OutboxSaga.Shipping.Application.Abstractions.Persistence;

public interface IShippingRepository
{
    Task AddAsync(Domain.Aggregates.ShippingAggregate.Shipping shipping, CancellationToken ct = default);
    Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken ct = default);
}
