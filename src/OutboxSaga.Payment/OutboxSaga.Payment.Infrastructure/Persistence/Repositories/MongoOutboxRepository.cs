using MongoDB.Driver;
using OutboxSaga.Payment.Application.Abstractions.Messaging;
using OutboxSaga.Payment.Application.Messaging;

namespace OutboxSaga.Payment.Infrastructure.Persistence.Repositories;

public sealed class MongoOutboxRepository : IOutboxRepository
{
    private readonly MongoContext _context;

    public MongoOutboxRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken ct = default)
    {
        if (_context.Session is not null)
        {
            await _context.OutboxMessages.InsertOneAsync(_context.Session, message, cancellationToken: ct);
        }
        else
        {
            await _context.OutboxMessages.InsertOneAsync(message, cancellationToken: ct);
        }
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnpublishedAsync(int batchSize, CancellationToken ct = default)
    {
        var filter = Builders<OutboxMessage>.Filter.Eq(m => m.PublishedAtUtc, null);
        return await _context.OutboxMessages
            .Find(filter)
            .Limit(batchSize)
            .ToListAsync(ct);
    }

    public async Task MarkAsPublishedAsync(string messageId, DateTime publishedAtUtc, CancellationToken ct = default)
    {
        var filter = Builders<OutboxMessage>.Filter.Eq(m => m.Id, messageId);
        var update = Builders<OutboxMessage>.Update.Set(m => m.PublishedAtUtc, publishedAtUtc);
        await _context.OutboxMessages.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
