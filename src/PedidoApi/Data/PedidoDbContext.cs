using Microsoft.Azure.Cosmos;
using PedidoApi.Models;

namespace PedidoApi.Data;

public class PedidoDbContext
{
    private readonly Container _pedidosContainer;
    private readonly Container _outboxContainer;

    public PedidoDbContext(CosmosClient cosmosClient, string databaseName)
    {
        _pedidosContainer = cosmosClient.GetContainer(databaseName, "Pedidos");
        _outboxContainer = cosmosClient.GetContainer(databaseName, "OutboxMessages");
    }
}
