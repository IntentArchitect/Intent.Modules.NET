using Intent.RoslynWeaver.Attributes;
using MassTransit.RetryPolicy.Immediate.Application.Common.Eventing;
using MassTransit.RetryPolicy.Immediate.Infrastructure.Configuration;
using MassTransit.RetryPolicy.Immediate.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RetryPolicy.Immediate.Infrastructure
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