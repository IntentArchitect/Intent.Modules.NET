using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using CompositeMessageBus.Infrastructure.Eventing.AzureEventGridBehaviors;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class AzureEventGridConfiguration
    {
        public static IServiceCollection AddEventGridConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<AzureEventGridMessageBus>();
            services.AddSingleton<AzureEventGridMessageDispatcher>();
            services.AddSingleton<IAzureEventGridMessageDispatcher, AzureEventGridMessageDispatcher>();

            services.AddScoped<EventContext>();
            services.AddScoped<IEventContext, EventContext>(sp => sp.GetRequiredService<EventContext>());
            services.AddScoped<AzureEventGridPublisherPipeline>();
            services.AddScoped<AzureEventGridConsumerPipeline>();

            services.AddScoped<IAzureEventGridConsumerBehavior, InboundCloudEventBehavior>();
            services.Configure<AzureEventGridPublisherOptions>(options =>
            {
                options.AddTopic<MsgAzEvtGridEvent>(configuration["EventGrid:Topics:MsgAzEvtGrid:Key"]!, configuration["EventGrid:Topics:MsgAzEvtGrid:Endpoint"]!, configuration["EventGrid:Topics:MsgAzEvtGrid:Source"]!);
            });
            registry.Register<MsgAzEvtGridEvent, AzureEventGridMessageBus>();

            return services;
        }
    }
}