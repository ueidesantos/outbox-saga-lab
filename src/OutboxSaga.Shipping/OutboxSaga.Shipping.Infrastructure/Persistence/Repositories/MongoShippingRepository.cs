using MongoDB.Driver;
using OutboxSaga.Shipping.Application.Abstractions.Persistence;

namespace OutboxSaga.Shipping.Infrastructure.Persistence.Repositories;

public sealed class MongoShippingRepository : IShippingRepository
{
    private readonly MongoContext _context;

    public MongoShippingRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Domain.Aggregates.ShippingAggregate.Shipping shipping, CancellationToken ct = default)
    {
        if (_context.Session is not null)
        {
            await _context.Shippings.InsertOneAsync(_context.Session, shipping, cancellationToken: ct);
        }
        else
        {
            await _context.Shippings.InsertOneAsync(shipping, cancellationToken: ct);
        }
    }

    public async Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var filter = Builders<Domain.Aggregates.ShippingAggregate.Shipping>.Filter.Eq(s => s.OrderId, orderId);
        return await _context.Shippings.Find(filter).AnyAsync(ct);
    }
}
