using OutboxSaga.Api.Contracts;
using OutboxSaga.Application.Pedidos;
using OutboxSaga.Application.Pedidos.Commands;

namespace OutboxSaga.Api.Endpoints;

public static class PedidosEndpoints
{
    public static IEndpointRouteBuilder MapPedidosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/pedidos", async (CreatePedidoRequest req, CreatePedidoHandler handler, CancellationToken ct) =>
        {
            var cmd = new CreatePedidoCommand(req.ClienteId, req.ValorTotal);
            var pedidoId = await handler.HandleAsync(cmd, ct);
            return Results.Created($"/pedidos/{pedidoId}", new CreatePedidoResponse(pedidoId));
        });

        return app;
    }
}