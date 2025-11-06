using Payments.Domain.Entities;
using Payments.Domain.Enum;
using Payments.Domain.Event;

namespace Payments.Domain.Repositories;

public interface IPaymentsRepository : ICommit
{
    Task AtualizarStatus(PaymentCreatedEvent @event, PaymentStatus status);

    Task<PaymentsEnt> TrazPagamentos(Guid perfilId, Guid jogoId);
}
