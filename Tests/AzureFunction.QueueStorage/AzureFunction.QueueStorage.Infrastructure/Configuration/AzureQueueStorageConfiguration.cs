using System;
using AzureFunction.QueueStorage.Application.Common.Eventing;
using AzureFunction.QueueStorage.Infrastructure.Eventing;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConfiguration", Version = "1.0")]

namespace AzureFunction.QueueStorage.Infrastructure.Configuration
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
            services.RegisterQueueStorageConsumers(configuration, "cleanarchitecture-queuestorage-eventing-messages-createproductcommand");
            services.Configure<AzureQueueStorageSubscriptionOptions>(options =>
            {
                options.Add<CreateProductCommand, IIntegrationEventHandler<CreateProductCommand>>(GetQueueName<CreateProductCommand>(configuration));
            });
            services.AddSingleton<IAzureQueueStorageEventDispatcher, AzureQueueStorageEventDispatcher>();
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
        }
    }
}