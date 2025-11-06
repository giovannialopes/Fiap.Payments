using MassTransit;
using Payments.Domain.Event;
using Payments.Domain.Services.Interface;

namespace Payments.Domain.Consumer
{
    public class PaymentCreatedConsumer : IConsumer<PaymentCreatedEvent>
    {
        private readonly IPaymentService _paymentService;

        public PaymentCreatedConsumer(IPaymentService paymentService) {
            _paymentService = paymentService;
        }

        public async Task Consume(ConsumeContext<PaymentCreatedEvent> context) {
            await _paymentService.Consumer(context.Message, context.CancellationToken);
        }
    }
}
