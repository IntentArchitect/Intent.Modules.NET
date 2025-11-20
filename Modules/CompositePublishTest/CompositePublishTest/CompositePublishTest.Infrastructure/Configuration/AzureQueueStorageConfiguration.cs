using Azure.Storage.Queues;
using CompositePublishTest.Eventing.Messages;
using CompositePublishTest.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConfiguration", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for Azure Queue Storage message broker provider.
    /// </summary>
    public static class AzureQueueStorageConfiguration
    {
        /// <summary>
        /// Configures Azure Queue Storage as a message broker provider.
        /// </summary>
        public static IServiceCollection ConfigureAzureQueueStorage(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            // Register Queue Service client
            services.AddSingleton<QueueServiceClient>(sp =>
                new QueueServiceClient(configuration["AzureQueueStorage:ConnectionString"]!));

            // Configure publisher options and message routing
            services.AddOptions<AzureQueueStorageOptions>().Bind(configuration.GetSection("QueueStorage"));

            // Register message types in the central registry
            registry.Register<ClientCreatedEvent, AzureQueueStorageMessageBus>();

            // Register Queue Storage provider as scoped
            services.AddScoped<AzureQueueStorageMessageBus>();

            return services;
        }
    }
}
