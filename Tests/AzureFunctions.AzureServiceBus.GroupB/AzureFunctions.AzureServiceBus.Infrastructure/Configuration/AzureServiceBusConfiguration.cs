using System;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupA.Eventing.Messages;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using AzureFunctions.AzureServiceBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Infrastructure.Configuration
{
    public static class AzureServiceBusConfiguration
    {
        public static IServiceCollection ConfigureAzureServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IEventBus, AzureServiceBusEventBus>();
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<PublisherOptions>(options =>
            {
                options.Add<SpecificTopicOneMessageEvent>(configuration["AzureServiceBus:SpecificTopic"]!);
                options.Add<SpecificQueueOneMessageEvent>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<SpecificQueueTwoMessageEvent>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<SpecificTopicTwoMessageEvent>(configuration["AzureServiceBus:SpecificTopic"]!);
            });
            services.Configure<SubscriptionOptions>(options =>
            {
                options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>();
                options.Add<CreateOrgIntegrationCommand, IIntegrationEventHandler<CreateOrgIntegrationCommand>>();
            });
            return services;
        }
    }
}