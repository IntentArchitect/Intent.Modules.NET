using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Publisher.Eventing.Messages;
using TestMediator.Subscriber.Application.Common.Eventing;
using TestMediator.Subscriber.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace TestMediator.Subscriber.Infrastructure.Configuration
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
                    cfg.UseInMemoryOutbox(context);
                });
                x.AddInMemoryInboxOutbox();
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>)).Endpoint(config => config.InstanceId = "TestMediator-Subscriber");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderCancelledEvent>, OrderCancelledEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderCancelledEvent>, OrderCancelledEvent>)).Endpoint(config => config.InstanceId = "TestMediator-Subscriber");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderShippedEvent>, OrderShippedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderShippedEvent>, OrderShippedEvent>)).Endpoint(config => config.InstanceId = "TestMediator-Subscriber");
        }
    }
}