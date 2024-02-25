using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;
using Subscribe.MassTransit.OutboxEF.Infrastructure.Eventing;
using Subscribe.MassTransit.OutboxEF.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Infrastructure.Configuration
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
                });
                x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderUpdatedEvent>, OrderUpdatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderUpdatedEvent>, OrderUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderDeletedEvent>, OrderDeletedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderDeletedEvent>, OrderDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<UserCreatedEvent>, UserCreatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<UserCreatedEvent>, UserCreatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<UserUpdatedEvent>, UserUpdatedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<UserUpdatedEvent>, UserUpdatedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<UserDeletedEvent>, UserDeletedEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<UserDeletedEvent>, UserDeletedEvent>)).Endpoint(config => config.InstanceId = "Subscribe-MassTransit-OutboxEF");
        }
    }
}