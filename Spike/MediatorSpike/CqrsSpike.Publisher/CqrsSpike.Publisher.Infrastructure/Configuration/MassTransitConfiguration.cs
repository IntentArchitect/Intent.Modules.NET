using System.Reflection;
using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Infrastructure.Eventing;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace CqrsSpike.Publisher.Infrastructure.Configuration
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
                    EndpointConventionRegistration();
                });
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<ShipmentCreatedEvent>, ShipmentCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<ShipmentCreatedEvent>, ShipmentCreatedEvent>)).Endpoint(config => config.InstanceId = "CqrsSpike-Publisher");
        }

        private static void EndpointConventionRegistration()
        {
            EndpointConvention.Map<CqrsSpike.Subscriber.Eventing.Messages.RequestShipmentCommand>(new Uri("queue:cqrs-spike.subscriber.eventing.messages.request-shipment-command"));
        }
    }
}