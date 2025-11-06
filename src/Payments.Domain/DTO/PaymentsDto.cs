namespace Payments.Domain.DTO;

public class PaymentsDto
{
    public Guid PerfilId { get; set; }
    public Guid JogoId { get; set; }
    public string StatusPedido { get; set; } = string.Empty;
}
