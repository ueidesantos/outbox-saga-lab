using MongoDB.Driver;
using OutboxSaga.Orders.Application.Abstractions.Persistence;
using OutboxSaga.Orders.Domain.Aggregates.OrderAggregate;
using OutboxSaga.Orders.Infrastructure.Persistence;

namespace OutboxSaga.Orders.Infrastructure.Persistence.Repositories;

using OrderAggregate = OutboxSaga.Orders.Domain.Aggregates.OrderAggregate;

public sealed class MongoOrderRepository : IOrderRepository
{
    private readonly MongoContext _context;

    public MongoOrderRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderAggregate.Order order, CancellationToken ct = default)
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

    public async Task<OrderAggregate.Order?> GetByIdAsync(string id, string customerId, CancellationToken ct = default)
    {
        var filter = Builders<OrderAggregate.Order>.Filter.And(
            Builders<OrderAggregate.Order>.Filter.Eq(order => order.Id, id),
            Builders<OrderAggregate.Order>.Filter.Eq("Customer.Id", customerId)
        );

        return await _context.Orders
            .Find(filter)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<OrderAggregate.Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Orders
            .Find(Builders<OrderAggregate.Order>.Filter.Empty)
            .ToListAsync(ct);
    }
}
