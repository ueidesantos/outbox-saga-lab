using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutboxSaga.Domain.Pedidos;
using OutboxSaga.Infrastructure.Data;
using OutboxSaga.Infrastructure.Pedidos;

namespace OutboxSaga.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cosmosSection = configuration.GetSection("CosmosDb");
        var connectionString = cosmosSection["ConnectionString"]
            ?? throw new InvalidOperationException("CosmosDb:ConnectionString não configurada.");
        var databaseName = cosmosSection["DatabaseName"]
            ?? throw new InvalidOperationException("CosmosDb:DatabaseName não configurada.");

        services.AddDbContext<OutboxSagaDbContext>(options =>
            options.UseCosmos(connectionString, databaseName));

        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
