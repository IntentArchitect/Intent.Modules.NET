using CleanArchitecture.Dapr.InvocationClient.Application.Common.Eventing;
using CleanArchitecture.Dapr.InvocationClient.Infrastructure.Configuration;
using CleanArchitecture.Dapr.InvocationClient.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEventBus, DaprMessageBus>();
            services.AddHttpClients(configuration);
            return services;
        }
    }
}