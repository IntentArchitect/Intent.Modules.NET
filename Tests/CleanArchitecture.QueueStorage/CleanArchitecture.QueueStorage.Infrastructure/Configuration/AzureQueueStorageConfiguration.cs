using AzureFunction.QueueStorage.Eventing.Messages;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using CleanArchitecture.QueueStorage.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConfiguration", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Configuration
{
    public static class AzureQueueStorageConfiguration
    {
        public static IServiceCollection AddQueueStorageConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<AzureQueueStorageMessageBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<AzureQueueStorageMessageBus>());
            services.AddOptions<AzureQueueStorageOptions>().Bind(configuration.GetSection("QueueStorage"));
            services.RegisterQueueStorageConsumers(configuration, "azurefunction-queuestorage-eventing-messages-productcreatedevent");
            services.RegisterQueueStorageConsumers(configuration, "cleanarchitecture-queuestorage-eventing-messages-createstockforproductcommand");
            services.Configure<AzureQueueStorageSubscriptionOptions>(options =>
            {
                options.Add<ProductCreatedEvent, IIntegrationEventHandler<ProductCreatedEvent>>(GetQueueName<ProductCreatedEvent>(configuration));
                options.Add<CreateStockForProductCommand, IIntegrationEventHandler<CreateStockForProductCommand>>(GetQueueName<CreateStockForProductCommand>(configuration));
            });
            services.AddHostedService<AzureQueueStorageBackgroundService>();
            return services;
        }

        private static string GetQueueName<T>(IConfiguration configuration)
        {
            return configuration[$"QueueStorage:QueueTypeMap:{typeof(T).FullName}"] ?? throw new ArgumentNullException($"No type -> queue mapping for '{typeof(T).FullName}'");
        }

        private static void RegisterQueueStorageConsumers(
            this IServiceCollection services,
            IConfiguration configuration,
            string queueName)
        {
            var queue = new QueueDefinition();
            configuration.GetSection($"QueueStorage:Queues:{queueName}").Bind(queue);
            ArgumentNullException.ThrowIfNull(queue);
            queue.QueueName = queueName;
            queue.Endpoint = string.IsNullOrWhiteSpace(queue.Endpoint) ? configuration["QueueStorage:DefaultEndpoint"] : queue.Endpoint;
            services.AddTransient<IAzureQueueStorageConsumer>(sp => ActivatorUtilities.CreateInstance<AzureQueueStorageConsumer>(sp, queue));
        }
    }
}