namespace Payments.Domain.Entities;

public class PaymentsEnt
{
    public long Id { get; set; }
    public Guid PerfilId { get; set; }
    public Guid JogoId { get; set; }
    public string StatusPedido { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdate { get; set; }
}
