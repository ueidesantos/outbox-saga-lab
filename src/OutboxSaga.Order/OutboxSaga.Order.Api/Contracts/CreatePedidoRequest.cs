namespace OutboxSaga.Api.Contracts;

public sealed record CreatePedidoRequest(
    string ClienteId,
    decimal ValorTotal
);