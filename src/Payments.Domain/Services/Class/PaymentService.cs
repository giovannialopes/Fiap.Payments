using Payments.Domain.DTO;
using Payments.Domain.Entities;
using Payments.Domain.Enum;
using Payments.Domain.Event;
using Payments.Domain.Repositories;
using Payments.Domain.Results;
using Payments.Domain.Services.Interface;
using Payments.Domain.Shared.Http;
using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Services.Class;

public class PaymentService(IPaymentsRepository paymentsRepository) : IPaymentService
{
    public async Task Consumer(PaymentCreatedEvent @event, CancellationToken cancellationToken) {
        if (@event.PerfilId == Guid.Empty)
            return;

        await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Pending);

        await paymentsRepository.AtualizarStatus(@event, PaymentStatus.AddingGame);

        await AdicionarJogo(@event);

        await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Waiting);

        await paymentsRepository.AtualizarStatus(@event, PaymentStatus.RemovingBalance);

        await RemoverSaldo(@event);

        await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Completed);
    }



    public async Task<Result<PaymentsDto>> ConsultarProcessos(Guid perfilId, Guid jogoId) {

        var pagamento = await paymentsRepository.TrazPagamentos(perfilId, jogoId);

        if (pagamento == null)
            return Result.Failure<PaymentsDto>("Nenhum pagamento encontrado para este usuário e jogo.", "404");

        var response = new PaymentsDto {
            PerfilId = pagamento.PerfilId,
            JogoId = pagamento.JogoId,
            StatusPedido = pagamento.StatusPedido,
        };

        return Result.Success(response);
    }






    private async Task RemoverSaldo(PaymentCreatedEvent @event) {
        var dto = new CarteiraDto.CarteiraDtoRequest {
            PerfilId = @event.PerfilId,
            Saldo = @event.saldo
        };

        Users.RemoverSaldo(dto);
    }

    private async Task AdicionarJogo(PaymentCreatedEvent @event) {
        var dto = new LibraryDto.LibraryDtoRequest {
            PerfilId = @event.PerfilId,
            JogoId = @event.JogoId
        };

        Games.InserirJogo(dto);
    }


}
