using MongoDB.Driver;
using OutboxSaga.Orders.Application.Abstractions.Persistence;
using OutboxSaga.Orders.Domain.Aggregates.OrderAggregate;
using OutboxSaga.Orders.Infrastructure.Persistence;

namespace OutboxSaga.Orders.Infrastructure.Persistence.Repositories;

public sealed class MongoOrderRepository : IOrderRepository
{
    private readonly MongoContext _context;

    public MongoOrderRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxSaga.Orders.Domain.Aggregates.OrderAggregate.Order order, CancellationToken ct = default)
    {
        if (_context.Session is not null)
        {
            await _context.Orders.InsertOneAsync(_context.Session, order, cancellationToken: ct);
        }
        else
        {
            await _context.Orders.InsertOneAsync(order, cancellationToken: ct);
        }
    }

    public async Task<OutboxSaga.Orders.Domain.Aggregates.OrderAggregate.Order?> GetByIdAsync(string id, string tenantId, CancellationToken ct = default)
    {
        var filter = Builders<OutboxSaga.Orders.Domain.Aggregates.OrderAggregate.Order>.Filter.Eq(order => order.Id, id);

        return await _context.Orders
            .Find(filter)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<OutboxSaga.Orders.Domain.Aggregates.OrderAggregate.Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Orders
            .Find(Builders<OutboxSaga.Orders.Domain.Aggregates.OrderAggregate.Order>.Filter.Empty)
            .ToListAsync(ct);
    }
}
