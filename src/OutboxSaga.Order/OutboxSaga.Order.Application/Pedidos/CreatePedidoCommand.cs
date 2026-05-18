namespace OutboxSaga.Application.Pedidos.Commands;
public sealed record CreatePedidoCommand(
    string ClienteId,
    decimal ValorTotal
);