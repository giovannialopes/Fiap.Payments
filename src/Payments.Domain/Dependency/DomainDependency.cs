using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Services.Class;
using Payments.Domain.Services.Interface;
using Payments.Domain.Shared.Http.Class;
using Payments.Domain.Shared.Http.Interface;

namespace Payments.Domain.Dependency;

public static class DomainDependency
{
    public static IServiceCollection AddServices(this IServiceCollection services) {
        return services
            .AddScoped<ILoggerServices, LoggerServices>()
            .AddScoped<IPaymentService, PaymentService>()
            // HTTP Clients para serviços externos - SOLID (DIP)
            .AddScoped<IGamesHttpClient, GamesHttpClient>()
            .AddScoped<IUsersHttpClient, UsersHttpClient>()
            // Observabilidade - SOLID (DIP)
            .AddSingleton<IMetricsService, MetricsService>()
            .AddSingleton<IPrometheusFormatter, PrometheusFormatter>()
            .AddSingleton<IRequestLogger, RequestLogger>();
    }
}

