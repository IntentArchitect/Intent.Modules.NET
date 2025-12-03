using System;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WindowsServiceHost.Tests.Common.Eventing;
using WindowsServiceHost.Tests.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace WindowsServiceHost.Tests.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection AddAzureServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddScoped<AzureServiceBusMessageBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<AzureServiceBusMessageBus>());
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            return services;
        }
    }
}