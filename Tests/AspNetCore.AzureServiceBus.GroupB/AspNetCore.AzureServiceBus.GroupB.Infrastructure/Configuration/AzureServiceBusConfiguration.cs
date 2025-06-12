using AspNetCore.AzureServiceBus.GroupA.Eventing.Messages;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Infrastructure.Eventing;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Infrastructure.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection ConfigureAzureServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddScoped<IEventBus, AzureServiceBusEventBus>();
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<AzureServiceBusSubscriptionOptions>(options =>
            {
                options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>(configuration["AzureServiceBus:ClientCreated"]!, configuration["AzureServiceBus:ClientCreatedSubscription"]);
            });
            return services;
        }
    }
}