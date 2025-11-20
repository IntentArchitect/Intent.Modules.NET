using Azure.Messaging.ServiceBus;
using CompositePublishTest.Eventing.Messages;
using CompositePublishTest.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for Azure Service Bus message broker provider.
    /// </summary>
    public static class AzureServiceBusConfiguration
    {
        /// <summary>
        /// Configures Azure Service Bus as a message broker provider.
        /// </summary>
        public static IServiceCollection ConfigureAzureServiceBus(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            // Register Service Bus client
            services.AddSingleton<ServiceBusClient>(sp =>
                new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]!));

            // Configure publisher options and message routing
            services.Configure<AzureServiceBusPublisherOptions>(options =>
            {
                options.Add<ClientCreatedEvent>(configuration["AzureServiceBus:ClientCreated"]!);
            });

            // Register message types in the central registry
            registry.Register<ClientCreatedEvent, AzureServiceBusMessageBus>();

            // Register Service Bus provider as scoped
            services.AddScoped<AzureServiceBusMessageBus>();

            return services;
        }
    }
}
