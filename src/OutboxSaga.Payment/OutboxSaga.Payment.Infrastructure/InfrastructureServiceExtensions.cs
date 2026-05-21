using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OutboxSaga.Payment.Application.Abstractions.Messaging;
using OutboxSaga.Payment.Application.Abstractions.Persistence;
using OutboxSaga.Payment.Infrastructure.Persistence;
using OutboxSaga.Payment.Infrastructure.Persistence.Repositories;

namespace OutboxSaga.Payment.Infrastructure;

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
        services.AddScoped<IPaymentRepository, MongoPaymentRepository>();
        services.AddScoped<IOutboxRepository, MongoOutboxRepository>();

        return services;
    }
}
