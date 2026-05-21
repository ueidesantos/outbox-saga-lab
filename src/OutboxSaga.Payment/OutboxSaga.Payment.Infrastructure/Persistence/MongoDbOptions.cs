namespace OutboxSaga.Payment.Infrastructure.Persistence;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = "PaymentDb";
}

public static class MongoCollectionNames
{
    public const string Payments = "payments";
    public const string OutboxMessages = "outbox_messages";
}
