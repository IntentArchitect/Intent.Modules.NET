using Azure.Messaging.ServiceBus;
using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection AddAzureServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddScoped<AzureServiceBusMessageBus>();
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<AzureServiceBusPublisherOptions>(options =>
            {
                options.Add<MsgAzSrvBusEvent>(configuration["AzureServiceBus:MsgAzSrvBus"]!);
            });
            registry.Register<MsgAzSrvBusEvent, AzureServiceBusMessageBus>();
            return services;
        }
    }
}