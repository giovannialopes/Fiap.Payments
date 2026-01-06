using Games.Domain.Queue.Event;
using Payments.Domain.DTO;
using Payments.Domain.Enum;
using Payments.Domain.Repositories;
using Payments.Domain.Results;
using Payments.Domain.Services.Interface;
using Payments.Domain.Shared.Http.Dto;
using Payments.Domain.Shared.Http.Interface;

namespace Payments.Domain.Services.Class;

public class PaymentService(
    IPaymentsRepository paymentsRepository, 
    IMetricsService metricsService,
    IGamesHttpClient gamesHttpClient,
    IUsersHttpClient usersHttpClient) : IPaymentService
{
    public async Task Consumer(PaymentCreatedEvent @event, CancellationToken cancellationToken) {
        // Validação de entrada
        if (@event == null || @event.PerfilId == Guid.Empty || @event.JogoId == Guid.Empty)
        {
            metricsService.IncrementPaymentsFailed();
            return;
        }

        try
        {
            metricsService.IncrementPaymentsProcessed();

            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Pending);

            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.AddingGame);
            await AdicionarJogo(@event);

            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Waiting);

            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.RemovingBalance);
            await RemoverSaldo(@event);

            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Completed);
            
            metricsService.IncrementPaymentsCompleted();
        }
        catch (Exception)
        {
            metricsService.IncrementPaymentsFailed();
            await paymentsRepository.AtualizarStatus(@event, PaymentStatus.Failed);
            throw;
        }
    }



    public async Task<Result<PaymentsDto>> ConsultarProcessos(Guid perfilId, Guid jogoId) {
        
        // Validação de entrada
        if (perfilId == Guid.Empty)
        {
            return Result.Failure<PaymentsDto>("PerfilId é obrigatório.", "400");
        }

        if (jogoId == Guid.Empty)
        {
            return Result.Failure<PaymentsDto>("JogoId é obrigatório.", "400");
        }

        var pagamento = await paymentsRepository.TrazPagamentos(perfilId, jogoId);

        if (pagamento == null)
        {
            return Result.Failure<PaymentsDto>("Nenhum pagamento encontrado para este usuário e jogo.", "404");
        }

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

        await Task.Run(() => usersHttpClient.RemoverSaldo(dto));
    }

    private async Task AdicionarJogo(PaymentCreatedEvent @event) {
        var dto = new LibraryDto.LibraryDtoRequest {
            PerfilId = @event.PerfilId,
            JogoId = @event.JogoId
        };
        await Task.Run(() => gamesHttpClient.InserirJogo(dto));
    }

}
