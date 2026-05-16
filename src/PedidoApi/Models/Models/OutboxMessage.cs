namespace PedidoApi.Models;

public class OutboxMessage
{
    public string Id { get; set; } = string.Empty;
    public string PedidoId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
}