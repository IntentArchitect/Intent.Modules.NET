using Intent.RoslynWeaver.Attributes;
using MassTransit.RetryPolicy.Incremental.Application.Common.Eventing;
using MassTransit.RetryPolicy.Incremental.Infrastructure.Configuration;
using MassTransit.RetryPolicy.Incremental.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RetryPolicy.Incremental.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MassTransitEventBus>();
            services.AddTransient<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}