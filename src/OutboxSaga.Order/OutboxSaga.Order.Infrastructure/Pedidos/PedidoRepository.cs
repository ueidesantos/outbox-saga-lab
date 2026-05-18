using Microsoft.EntityFrameworkCore;
using OutboxSaga.Domain.Pedidos;
using OutboxSaga.Infrastructure.Data;

namespace OutboxSaga.Infrastructure.Pedidos;

public sealed class PedidoRepository(OutboxSagaDbContext db) : IPedidoRepository
{
    public async Task AddAsync(Pedido pedido, CancellationToken ct = default)
    {
        pedido.CriadoEm = DateTime.UtcNow;
        db.Pedidos.Add(pedido);

        await db.SaveChangesAsync(ct);
    }


    public async Task<Pedido?> GetByIdAsync(string id, string clienteId, CancellationToken ct = default)
    {
        return await db.Pedidos
            .WithPartitionKey(clienteId)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Pedido>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Pedidos.ToListAsync(ct);
    }
}
