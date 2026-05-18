using OutboxSaga.Application.Pedidos.Commands;
using OutboxSaga.Domain.Pedidos;

namespace OutboxSaga.Application.Pedidos;

public sealed class CreatePedidoHandler
{
    private readonly IPedidoRepository _pedidoRepository;

    public CreatePedidoHandler(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<string> HandleAsync(CreatePedidoCommand command, CancellationToken ct)
    {
        var pedidoId = Guid.NewGuid().ToString("N");

        var pedido = new Pedido(
            clienteId: command.ClienteId,
            descricao: $"Pedido {pedidoId} para cliente {command.ClienteId}",
            valorTotal: command.ValorTotal
        );

        await _pedidoRepository.AddAsync(pedido, ct);

        return pedidoId;
    }
}