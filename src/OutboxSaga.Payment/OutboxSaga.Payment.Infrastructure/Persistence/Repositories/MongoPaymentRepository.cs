using MongoDB.Driver;
using OutboxSaga.Payment.Application.Abstractions.Persistence;

namespace OutboxSaga.Payment.Infrastructure.Persistence.Repositories;

public sealed class MongoPaymentRepository : IPaymentRepository
{
    private readonly MongoContext _context;

    public MongoPaymentRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Domain.Aggregates.PaymentAggregate.Payment payment, CancellationToken ct = default)
    {
        if (_context.Session is not null)
        {
            await _context.Payments.InsertOneAsync(_context.Session, payment, cancellationToken: ct);
        }
        else
        {
            await _context.Payments.InsertOneAsync(payment, cancellationToken: ct);
        }
    }

    public async Task<bool> ExistsForOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var filter = Builders<Domain.Aggregates.PaymentAggregate.Payment>.Filter.Eq(p => p.OrderId, orderId);
        return await _context.Payments.Find(filter).AnyAsync(ct);
    }
}
