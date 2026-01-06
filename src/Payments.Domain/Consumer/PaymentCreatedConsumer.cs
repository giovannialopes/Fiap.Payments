using Games.Domain.Queue.Event;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payments.Domain.Services.Interface;

namespace Payments.Domain.Consumer
{
    public class PaymentCreatedConsumer : IConsumer<PaymentCreatedEvent>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentCreatedConsumer> _logger;

        public PaymentCreatedConsumer(IPaymentService paymentService, ILogger<PaymentCreatedConsumer> logger) {
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCreatedEvent> context) {
            _logger.LogInformation("[PAYMENTS] Mensagem recebida: {@Event}", context.Message);

            try {
                await _paymentService.Consumer(context.Message, context.CancellationToken);
                _logger.LogInformation("[PAYMENTS] Processado com sucesso: JogoId={JogoId}, PerfilId={PerfilId}", context.Message.JogoId, context.Message.PerfilId);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "[PAYMENTS] Erro ao processar PaymentCreatedEvent: JogoId={JogoId}, PerfilId={PerfilId}", context.Message.JogoId, context.Message.PerfilId);
                throw;
            }
        }
    }
}
