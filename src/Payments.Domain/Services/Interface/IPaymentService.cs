using Payments.Domain.DTO;
using Games.Domain.Queue.Event;
using Payments.Domain.Results;
using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Services.Interface
{
    public interface IPaymentService
    {
        Task Consumer(PaymentCreatedEvent @event, CancellationToken cancellationToken);
        Task<Result<PaymentsDto>> ConsultarProcessos(Guid perfilId, Guid jogoId);
    }
}
