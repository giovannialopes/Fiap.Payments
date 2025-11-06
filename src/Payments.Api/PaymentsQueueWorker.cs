using Azure.Messaging.ServiceBus;
using Payments.Domain.Event;
using Payments.Domain.Services.Interface;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class PaymentsQueueWorker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly JsonSerializerOptions _json = new() {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public PaymentsQueueWorker(ServiceBusProcessor processor, IServiceScopeFactory scopeFactory) {
        _processor = processor;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        Console.WriteLine("[PAYMENTS] Escutando fila do Service Bus...");

        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += OnErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args) {
        try {
            var json = args.Message.Body.ToString();
            Console.WriteLine($"[PAYMENTS] Mensagem recebida: {json}");

            var evt = JsonSerializer.Deserialize<PaymentCreatedEvent>(json, _json);
            if (evt == null) {
                Console.WriteLine("[PAYMENTS] Payload inválido");
                await args.AbandonMessageAsync(args.Message);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            await service.Consumer(evt, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message);
            Console.WriteLine("[PAYMENTS] Processado com sucesso");
        }
        catch (Exception ex) {
            Console.WriteLine($"[PAYMENTS] Erro ao processar: {ex.Message}");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args) {
        Console.WriteLine($"[PAYMENTS][ERRO] {args.Exception.Message} | Entity={args.EntityPath}");
        return Task.CompletedTask;
    }
}
