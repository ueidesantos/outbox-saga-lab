using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OutboxSaga.Shipping.Application.Abstractions.Messaging;
using OutboxSaga.Shipping.Application.Abstractions.Persistence;
using OutboxSaga.Shipping.Infrastructure.Persistence;
using OutboxSaga.Shipping.Infrastructure.Persistence.Repositories;

namespace OutboxSaga.Shipping.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoOptions = configuration
            .GetSection("MongoDb")
            .Get<MongoDbOptions>()
            ?? throw new InvalidOperationException("MongoDb configuration is required.");

        services.AddSingleton(mongoOptions);
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoOptions.ConnectionString));
        services.AddScoped<MongoContext>();
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
        services.AddScoped<IShippingRepository, MongoShippingRepository>();
        services.AddScoped<IOutboxRepository, MongoOutboxRepository>();

        return services;
    }
}
