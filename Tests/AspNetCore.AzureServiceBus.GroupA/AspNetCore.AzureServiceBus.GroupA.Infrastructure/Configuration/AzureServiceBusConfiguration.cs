using AspNetCore.AzureServiceBus.GroupA.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupA.Eventing.Messages;
using AspNetCore.AzureServiceBus.GroupA.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupA.Infrastructure.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection ConfigureAzureServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IEventBus, AzureServiceBusEventBus>();
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<PublisherOptions>(options =>
            {
                options.Add<ClientCreatedEvent>(configuration["AzureServiceBus:ClientCreated"]!);
            });
            return services;
        }
    }
}