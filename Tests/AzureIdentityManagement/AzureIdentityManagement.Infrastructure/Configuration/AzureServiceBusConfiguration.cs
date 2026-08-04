using Azure.Identity;
using Azure.Messaging.ServiceBus;
using AzureIdentityManagement.Application.Common.Eventing;
using AzureIdentityManagement.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace AzureIdentityManagement.Infrastructure.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection AddAzureServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (string.Equals(configuration["AzureServiceBus:AuthenticationMethod"], "managed-identity", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:FullyQualifiedNamespace"], new DefaultAzureCredential()));
            }
            else
            {
                services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            }
            services.AddScoped<AzureServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<AzureServiceBusMessageBus>());
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            return services;
        }
    }
}