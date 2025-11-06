namespace Payments.Domain.Event;

public record PaymentCreatedEvent(
Guid JogoId,
Guid PerfilId,
decimal saldo
);