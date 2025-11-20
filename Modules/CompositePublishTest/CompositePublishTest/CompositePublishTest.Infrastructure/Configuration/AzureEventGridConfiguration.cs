using CompositePublishTest.Application.Common.Eventing;
using CompositePublishTest.Eventing.Messages;
using CompositePublishTest.Infrastructure.Eventing;
using CompositePublishTest.Infrastructure.Eventing.AzureEventGridBehaviors;
using CompositePublishTest.Infrastructure.Eventing.Dispatchers;
using CompositePublishTest.Infrastructure.Eventing.Models;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridConfiguration", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Configuration
{
    public static class AzureEventGridConfiguration
    {
        public static IServiceCollection ConfigureEventGrid(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddSingleton<AzureEventGridMessageDispatcher>();
            services.AddSingleton<IAzureEventGridMessageDispatcher, AzureEventGridMessageDispatcher>();

            services.AddScoped<EventContext>();
            services.AddScoped<IEventContext, EventContext>(sp => sp.GetRequiredService<EventContext>());
            services.AddScoped<Eventing.AzureEventGridBehaviors.AzureEventGridPublisherPipeline>();
            services.AddScoped<Eventing.AzureEventGridBehaviors.AzureEventGridConsumerPipeline>();

            services.AddScoped<Eventing.AzureEventGridBehaviors.IAzureEventGridConsumerBehavior, InboundCloudEventBehavior>();

            // Configure publisher options and message routing
            services.Configure<AzureEventGridPublisherOptions>(options =>
            {
                options.AddTopic<ClientCreatedEvent>(
                    configuration["EventGrid:Topics:ClientCreatedEvent:Key"]!,
                    configuration["EventGrid:Topics:ClientCreatedEvent:Endpoint"]!,
                    configuration["EventGrid:Topics:ClientCreatedEvent:Source"]!);
            });

            // Register message types in the central registry
            registry.Register<ClientCreatedEvent, AzureEventGridMessageBus>();

            // Register Event Grid provider as scoped
            services.AddScoped<AzureEventGridMessageBus>();

            return services;
        }
    }
}