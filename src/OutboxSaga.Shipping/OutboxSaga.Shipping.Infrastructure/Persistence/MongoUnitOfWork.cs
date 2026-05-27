using MongoDB.Driver;
using OutboxSaga.Shipping.Application.Abstractions.Persistence;

namespace OutboxSaga.Shipping.Infrastructure.Persistence;

public sealed class MongoUnitOfWork : IUnitOfWork
{
    private readonly IMongoClient _mongoClient;
    private readonly MongoContext _context;

    public MongoUnitOfWork(IMongoClient mongoClient, MongoContext context)
    {
        _mongoClient = mongoClient;
        _context = context;
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        using var session = await _mongoClient.StartSessionAsync(cancellationToken: ct);
        _context.UseSession(session);

        try
        {
            session.StartTransaction();
            await action(ct);
            await session.CommitTransactionAsync(ct);
        }
        catch
        {
            await session.AbortTransactionAsync(ct);
            throw;
        }
        finally
        {
            _context.ClearSession();
        }
    }
}
