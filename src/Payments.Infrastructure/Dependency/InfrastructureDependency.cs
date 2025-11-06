using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Repositories;

namespace Payments.Infrastructure.Dependency;

public static class InfrastructureDependency
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) {
        return services
            .AddScoped<ILoggerRepository, LoggerRepository>()
            .AddScoped<IPaymentsRepository, PaymentsRepository>();


    }

}