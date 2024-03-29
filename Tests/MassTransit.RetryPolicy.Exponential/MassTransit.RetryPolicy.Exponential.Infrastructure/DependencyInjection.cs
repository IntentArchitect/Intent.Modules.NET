using Intent.RoslynWeaver.Attributes;
using MassTransit.RetryPolicy.Exponential.Application.Common.Eventing;
using MassTransit.RetryPolicy.Exponential.Infrastructure.Configuration;
using MassTransit.RetryPolicy.Exponential.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RetryPolicy.Exponential.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}