using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Eventing.Messages;
using AzureFunctions.AzureEventGrid.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridConfiguration", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Configuration
{
    public static class AzureEventGridConfiguration
    {
        public static IServiceCollection ConfigureEventGrid(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEventBus, AzureEventGridEventBus>();
            services.AddSingleton<AzureEventGridMessageDispatcher>();
            services.AddSingleton<IAzureEventGridMessageDispatcher, AzureEventGridMessageDispatcher>();
            services.Configure<PublisherOptions>(options =>
            {
                options.Add<SpecificTopicOneMessageEvent>(configuration["EventGrid:Topics:SpecificTopic:Key"]!, configuration["EventGrid:Topics:SpecificTopic:Endpoint"]!);
                options.Add<SpecificTopicTwoMessageEvent>(configuration["EventGrid:Topics:SpecificTopic:Key"]!, configuration["EventGrid:Topics:SpecificTopic:Endpoint"]!);
                options.Add<ClientCreatedEvent>(configuration["EventGrid:Topics:ClientCreatedEvent:Key"]!, configuration["EventGrid:Topics:ClientCreatedEvent:Endpoint"]!);
            });
            services.Configure<SubscriptionOptions>(options =>
            {
                options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>();
                options.Add<SpecificTopicOneMessageEvent, IIntegrationEventHandler<SpecificTopicOneMessageEvent>>();
                options.Add<SpecificTopicTwoMessageEvent, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>>();
            });
            return services;
        }
    }
}