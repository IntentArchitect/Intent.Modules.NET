using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.DaprConfiguration", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure.Configuration
{
    public static class DaprConfiguration
    {
        public static IServiceCollection AddDaprConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<DaprEventBus>();

            services.AddScoped<IEventBus, DaprEventBus>();

            return services;
        }
    }
}