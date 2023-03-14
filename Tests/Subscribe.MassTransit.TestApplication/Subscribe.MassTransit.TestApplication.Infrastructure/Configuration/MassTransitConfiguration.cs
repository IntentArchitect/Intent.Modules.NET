using System.Reflection;
using Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.TestApplication.Application.Common.Eventing;
using Subscribe.MassTransit.TestApplication.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Subscribe.MassTransit.TestApplication.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                AddConsumers(x);

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });

            });
        }

        private static void AddConsumers(IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<EventStartedEvent>, EventStartedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<EventStartedEvent>, EventStartedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-TestApplication");
        }
    }
}