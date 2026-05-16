namespace PedidoApi.Models;

public class Pedido
{
    public string Id { get; set; } = string.Empty;
    public string PedidoId { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
