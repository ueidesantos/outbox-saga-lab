namespace OutboxSaga.Domain.Pedidos;

public class Pedido
{
    public Pedido(string clienteId, string descricao, decimal valorTotal)
    {
        Id = Guid.NewGuid().ToString();
        this.clienteId = clienteId;
        Descricao = descricao;
        ValorTotal = valorTotal;
    }

    public string Id { get; set; } = string.Empty;
    public string clienteId { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public DateTime? CriadoEm { get; set; }
}
