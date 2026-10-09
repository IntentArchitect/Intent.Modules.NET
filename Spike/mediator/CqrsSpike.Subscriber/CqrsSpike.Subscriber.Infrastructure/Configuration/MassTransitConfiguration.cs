using System.Reflection;
using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Eventing.Messages;
using CqrsSpike.Subscriber.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace CqrsSpike.Subscriber.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<MassTransitMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<MassTransitMessageBus>());

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], host =>
                    {
                        host.Username(configuration["RabbitMq:Username"]);
                        host.Password(configuration["RabbitMq:Password"]);
                    });

                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.AddReceiveEndpoints(context);
                });
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>)).Endpoint(config => config.InstanceId = "CqrsSpike-Subscriber");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<RequestShipmentCommand>, RequestShipmentCommand>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<RequestShipmentCommand>, RequestShipmentCommand>)).ExcludeFromConfigureEndpoints();
        }

        private static void AddReceiveEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
        {
            cfg.ReceiveEndpoint("cqrs-spike.subscriber.eventing.messages.request-shipment-command", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<RequestShipmentCommand>, RequestShipmentCommand>>(context);
            });
        }
    }
}