using Microsoft.EntityFrameworkCore;
using OutboxSaga.Domain.Pedidos;

namespace OutboxSaga.Infrastructure.Data;

public sealed class OutboxSagaDbContext(DbContextOptions<OutboxSagaDbContext> options) : DbContext(options)
{
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("Pedidos");

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToContainer("Pedidos");
            entity.HasPartitionKey(p => p.clienteId);
            entity.HasKey(p => p.Id);
            entity.Property(p => p.clienteId);
            entity.Property(p => p.Descricao);
            entity.Property(p => p.ValorTotal);
            entity.Property(p => p.CriadoEm);
        });
    }
}
