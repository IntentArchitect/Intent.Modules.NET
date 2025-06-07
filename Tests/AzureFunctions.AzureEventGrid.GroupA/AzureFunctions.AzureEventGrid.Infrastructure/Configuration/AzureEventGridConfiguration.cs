using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages;
using AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages;
using AzureFunctions.AzureEventGrid.Infrastructure.Eventing;
using AzureFunctions.AzureEventGrid.Infrastructure.Eventing.AzureEventGridBehaviors;
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

            services.AddScoped<EventContext>();
            services.AddScoped<IEventContext, EventContext>(sp => sp.GetRequiredService<EventContext>());
            services.AddScoped<AzureEventGridPublisherPipeline>();
            services.AddScoped<AzureEventGridConsumerPipeline>();

            services.AddScoped<IAzureEventGridConsumerBehavior, InboundCloudEventBehavior>();
            services.Configure<AzureEventGridPublisherOptions>(options =>
            {
                options.Add<ClientCreatedEvent>(configuration["EventGrid:Topics:ClientCreatedEvent:Key"]!, configuration["EventGrid:Topics:ClientCreatedEvent:Endpoint"]!, configuration["EventGrid:Topics:ClientCreatedEvent:Source"]!);
            });
            services.Configure<AzureEventGridSubscriptionOptions>(options =>
            {
                options.Add<SpecificTopicOneMessageEvent, IIntegrationEventHandler<SpecificTopicOneMessageEvent>>();
                options.Add<SpecificTopicTwoMessageEvent, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>>();
            });
            return services;
        }
    }
}