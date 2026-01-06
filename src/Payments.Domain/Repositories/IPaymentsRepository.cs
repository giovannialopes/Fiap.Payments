using Payments.Domain.Entities;
using Payments.Domain.Enum;
using Games.Domain.Queue.Event;

namespace Payments.Domain.Repositories;

public interface IPaymentsRepository : ICommit
{
    Task AtualizarStatus(PaymentCreatedEvent @event, PaymentStatus status);

    Task<PaymentsEnt> TrazPagamentos(Guid perfilId, Guid jogoId);
}
