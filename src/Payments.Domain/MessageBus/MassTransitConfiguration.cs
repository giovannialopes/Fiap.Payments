using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Consumer;

namespace Payments.Domain.MessageBus;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration) {
        services.AddMassTransit(x => {
            x.AddConsumer<PaymentCreatedConsumer>();

            x.UsingAzureServiceBus((context, cfg) => {
                cfg.Host(configuration["ServiceBus:ConnectionString"]);

                // Consumo direto de uma FILA (tier Basic)
                cfg.ReceiveEndpoint(configuration["ServiceBus:Queue"] ?? "payments", e => {
                    // Em Basic, evite tentar criar bindings em tópicos
                    e.ConfigureConsumeTopology = false;

                    // boas práticas de processamento
                    e.PrefetchCount = 16;
                    e.MaxAutoRenewDuration = TimeSpan.FromMinutes(10);
                    e.LockDuration = TimeSpan.FromSeconds(45);
                    e.MaxDeliveryCount = 10; // depois disso vai pra DLQ

                    e.ConfigureConsumer<PaymentCreatedConsumer>(context);
                });
            });
        });

        return services;
    }
}

