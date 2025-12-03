using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class AzureQueueStorageConfiguration
    {
        public static IServiceCollection AddQueueStorageConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<AzureQueueStorageMessageBus>();
            services.AddOptions<AzureQueueStorageOptions>().Bind(configuration.GetSection("QueueStorage"));
            services.Configure<AzureQueueStorageSubscriptionOptions>(options => { });
            registry.Register<MsgQStorageEvent, AzureQueueStorageMessageBus>();
            return services;
        }
    }
}