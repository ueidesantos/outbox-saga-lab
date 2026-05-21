using MongoDB.Driver;
using OutboxSaga.Payment.Application.Messaging;

namespace OutboxSaga.Payment.Infrastructure.Persistence;

public sealed class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IMongoClient mongoClient, MongoDbOptions options)
    {
        _database = mongoClient.GetDatabase(options.DatabaseName);
    }

    public IClientSessionHandle? Session { get; private set; }

    public IMongoCollection<Domain.Aggregates.PaymentAggregate.Payment> Payments
        => _database.GetCollection<Domain.Aggregates.PaymentAggregate.Payment>(MongoCollectionNames.Payments);

    public IMongoCollection<OutboxMessage> OutboxMessages
        => _database.GetCollection<OutboxMessage>(MongoCollectionNames.OutboxMessages);

    public void UseSession(IClientSessionHandle session)
    {
        Session = session;
    }

    public void ClearSession()
    {
        Session = null;
    }
}
