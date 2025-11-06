using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Services.Class;
using Payments.Domain.Services.Interface;

namespace Payments.Domain.Dependency;

public static class DomainDependency
{
    public static IServiceCollection AddServices(this IServiceCollection services) {
        return services
            .AddScoped<ILoggerServices, LoggerServices>()
            .AddScoped<IPaymentService, PaymentService>();
    }
}

