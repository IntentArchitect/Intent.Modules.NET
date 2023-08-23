using Intent.RoslynWeaver.Attributes;
using MassTransit.RetryPolicy.Interval.Application.Common.Eventing;
using MassTransit.RetryPolicy.Interval.Infrastructure.Configuration;
using MassTransit.RetryPolicy.Interval.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RetryPolicy.Interval.Infrastructure
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