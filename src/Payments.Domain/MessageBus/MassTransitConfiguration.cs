using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payments.Domain.Consumer;

namespace Payments.Domain.MessageBus;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration) {
        services.AddMassTransit(x => {
            x.AddConsumer<PaymentCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) => {
                var loggerFactory = context.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("MassTransit");
                
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var port = configuration.GetValue<ushort>("RabbitMq:Port", 5672);
                var virtualHost = configuration["RabbitMq:VirtualHost"] ?? "/";
                var username = configuration["RabbitMq:Username"] ?? "guest";
                var password = configuration["RabbitMq:Password"] ?? "guest";
                var queueName = configuration["RabbitMq:Queue"] ?? "payments";

                logger.LogInformation("Configurando RabbitMQ: Host={Host}, Port={Port}, VirtualHost={VirtualHost}, Queue={Queue}", 
                    host, port, virtualHost, queueName);

                cfg.Host(host, port, virtualHost, h => {
                    h.Username(username);
                    h.Password(password);
                });

                // Configuração de retry com backoff exponencial
                cfg.UseMessageRetry(r => r.Exponential(
                    retryLimit: 5,
                    minInterval: TimeSpan.FromSeconds(1),
                    maxInterval: TimeSpan.FromSeconds(30),
                    intervalDelta: TimeSpan.FromSeconds(2)
                ));

                // Configuração de reconexão automática
                cfg.UseCircuitBreaker(cb => {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ReceiveEndpoint(queueName, e => {
                    e.PrefetchCount = 16;
                    
                    // Configuração de retry específica do endpoint
                    e.UseMessageRetry(r => {
                        r.Exponential(
                            retryLimit: 3,
                            minInterval: TimeSpan.FromSeconds(1),
                            maxInterval: TimeSpan.FromSeconds(10),
                            intervalDelta: TimeSpan.FromSeconds(2)
                        );
                        // Após esgotar retries, move para DLQ automaticamente
                        r.Handle<Exception>();
                    });

                    // Configuração de erro - move para DLQ após retries
                    e.UseInMemoryOutbox();
                    
                    // Configuração de DLQ - RabbitMQ cria automaticamente com sufixo _error
                    e.Bind(queueName + "_error", s => {
                        s.Durable = true;
                        s.AutoDelete = false;
                    });
                    
                    e.ConfigureConsumer<PaymentCreatedConsumer>(context);
                });
            });
        });

        return services;
    }
}

