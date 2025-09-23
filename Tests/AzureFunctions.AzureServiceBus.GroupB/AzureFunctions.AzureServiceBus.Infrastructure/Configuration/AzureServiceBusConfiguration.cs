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
        public static IServiceCollection ConfigureAzureServiceBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddScoped<IEventBus, AzureServiceBusEventBus>();
            services.AddSingleton<AzureServiceBusMessageDispatcher>();
            services.AddSingleton<IAzureServiceBusMessageDispatcher, AzureServiceBusMessageDispatcher>();
            services.Configure<AzureServiceBusPublisherOptions>(options =>
            {
                options.Add<SpecificTopicOneMessageEvent>(configuration["AzureServiceBus:SpecificTopic"]!);
                options.Add<SpecificQueueOneMessageEvent>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<SpecificQueueTwoMessageEvent>(configuration["AzureServiceBus:SpecificQueue"]!);
                options.Add<PublishAndConsumeMessageEvent>(configuration["AzureServiceBus:PublishAndConsume"]!);
                options.Add<SpecificTopicTwoMessageEvent>(configuration["AzureServiceBus:SpecificTopic"]!);
            });
            services.Configure<AzureServiceBusSubscriptionOptions>(options =>
            {
                options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>(configuration["AzureServiceBus:ClientCreated"]!, configuration["AzureServiceBus:ClientCreatedSubscription"]);
                options.Add<PublishAndConsumeMessageEvent, IIntegrationEventHandler<PublishAndConsumeMessageEvent>>(configuration["AzureServiceBus:PublishAndConsume"]!, configuration["AzureServiceBus:PublishAndConsumeSubscription"]);
                options.Add<CreateOrgIntegrationCommand, IIntegrationEventHandler<CreateOrgIntegrationCommand>>(configuration["AzureServiceBus:CreateOrg"]!);
            });
            return services;
        }
    }
}