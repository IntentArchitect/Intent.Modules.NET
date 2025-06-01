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
                options.Add<ClientCreatedEvent>(configuration["AzureServiceBus:ClientCreated"]!);
                options.Add<CreateOrgIntegrationCommand>(configuration["AzureServiceBus:CreateOrg"]!);
            });
            services.Configure<SubscriptionOptions>(options =>
            {
                options.Add<SpecificQueueOneMessageEvent, IIntegrationEventHandler<SpecificQueueOneMessageEvent>>();
                options.Add<SpecificQueueTwoMessageEvent, IIntegrationEventHandler<SpecificQueueTwoMessageEvent>>();
                options.Add<SpecificTopicOneMessageEvent, IIntegrationEventHandler<SpecificTopicOneMessageEvent>>();
                options.Add<SpecificTopicTwoMessageEvent, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>>();
            });
            return services;
        }
    }
}