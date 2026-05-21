using MongoDB.Driver;
using OutboxSaga.Shipping.Application.Messaging;

namespace OutboxSaga.Shipping.Infrastructure.Persistence;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = "ShippingDb";
}

public static class MongoCollectionNames
{
    public const string Shippings = "shippings";
    public const string OutboxMessages = "outbox_messages";
}

public sealed class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IMongoClient mongoClient, MongoDbOptions options)
    {
        _database = mongoClient.GetDatabase(options.DatabaseName);
    }

    public IClientSessionHandle? Session { get; private set; }

    public IMongoCollection<Domain.Aggregates.ShippingAggregate.Shipping> Shippings
        => _database.GetCollection<Domain.Aggregates.ShippingAggregate.Shipping>(MongoCollectionNames.Shippings);

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
