using System;
using Azure.Messaging.ServiceBus;
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
        public static IServiceCollection AddAzureServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddScoped<AzureServiceBusEventBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<AzureServiceBusEventBus>());
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<AzureServiceBusPublisherOptions>(options =>
            {
                options.Add<ClientCreatedEvent>(configuration["AzureServiceBus:ClientCreated"]!);
                options.Add<CreateOrgIntegrationCommand>(configuration["AzureServiceBus:CreateOrg"]!);
            });
            services.Configure<AzureServiceBusSubscriptionOptions>(options =>
            {
                options.Add<SpecificQueueOneMessageEvent, IIntegrationEventHandler<SpecificQueueOneMessageEvent>>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<SpecificQueueTwoMessageEvent, IIntegrationEventHandler<SpecificQueueTwoMessageEvent>>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<SpecificTopicOneMessageEvent, IIntegrationEventHandler<SpecificTopicOneMessageEvent>>(configuration["AzureServiceBus:SpecificTopic"]!, configuration["AzureServiceBus:SpecificTopicSubscription"]);
                options.Add<SpecificTopicTwoMessageEvent, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>>(configuration["AzureServiceBus:SpecificTopic"]!, configuration["AzureServiceBus:SpecificTopicSubscription"]);
            });
            return services;
        }
    }
}