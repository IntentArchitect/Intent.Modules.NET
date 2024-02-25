using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxMemory.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MassTransitEventBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<MassTransitEventBus>());

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
                    cfg.AddMessageTopologyConfiguration();
                });
                x.AddInMemoryInboxOutbox();
            });
        }

        private static void AddMessageTopologyConfiguration(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Message<BasketCreatedEvent>(x => x.SetEntityName("basket-created-topic-rename"));
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<RoleCreatedEvent>, RoleCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<RoleUpdatedEvent>, RoleUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<RoleDeletedEvent>, RoleDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DelayedNotificationEvent>, DelayedNotificationEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
        }
    }
}