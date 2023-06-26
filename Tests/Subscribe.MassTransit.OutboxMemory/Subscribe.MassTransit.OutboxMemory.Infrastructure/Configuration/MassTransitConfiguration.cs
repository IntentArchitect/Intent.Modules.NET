using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
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
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                AddConsumers(x);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(10, TimeSpan.FromSeconds(30)));

                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], h =>
                    {
                        h.Username(configuration["RabbitMq:Username"]);
                        h.Password(configuration["RabbitMq:Password"]);
                    });

                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox();
                });

            });
        }

        private static void AddConsumers(IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketCreatedEvent>, BasketCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketUpdatedEvent>, BasketUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketDeletedEvent>, BasketDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemCreatedEvent>, BasketItemCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemUpdatedEvent>, BasketItemUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<BasketItemDeletedEvent>, BasketItemDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxMemory");
        }
    }
}