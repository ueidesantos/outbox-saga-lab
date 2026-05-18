namespace OutboxSaga.Domain.Pedidos;

public interface IPedidoRepository
{
    Task AddAsync(Pedido pedido, CancellationToken ct = default);
    Task<Pedido?> GetByIdAsync(string id, string clienteId, CancellationToken ct = default);
    Task<IReadOnlyList<Pedido>> GetAllAsync(CancellationToken ct = default);
}
