using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.CompositeMessageBusConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class CompositeMessageBusConfiguration
    {
        public static IServiceCollection ConfigureCompositeMessageBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var registry = new MessageBrokerRegistry();

            services.AddSqsConfiguration(configuration, registry);
            services.AddEventGridConfiguration(configuration, registry);
            services.AddQueueStorageConfiguration(configuration, registry);
            services.AddAzureServiceBusConfiguration(configuration, registry);
            services.AddKafkaConfiguration(configuration, registry);
            services.AddMassTransitConfiguration(configuration, registry);

            services.AddSolaceConfiguration(configuration, registry);

            services.AddSingleton(registry);
            services.AddScoped<MessageBrokerResolver>();
            services.AddScoped<IEventBus, Infrastructure.Eventing.CompositeMessageBus>();

            return services;
        }
    }
}