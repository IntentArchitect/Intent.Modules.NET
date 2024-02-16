using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Eventing.Messages;
using MassTransit.RabbitMQ.Infrastructure.Eventing;
using MassTransit.RabbitMQ.Services;
using MassTransit.RabbitMQ.Services.Animals;
using MassTransit.RabbitMQ.Services.People;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace MassTransit.RabbitMQ.Infrastructure.Configuration
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
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VirtualHost"], host =>
                    {
                        host.Username(configuration["RabbitMq:Username"]);
                        host.Password(configuration["RabbitMq:Password"]);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.ConfigureNonDefaultEndpoints(context);
                    cfg.AddReceiveEndpoints(context);
                    EndpointConventionRegistration();
                });
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>)).Endpoint(config => { config.InstanceId = "MassTransit-RabbitMQ"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>)).Endpoint(config => { config.InstanceId = "MassTransit-RabbitMQ"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>)).Endpoint(config => { config.InstanceId = "MassTransit-RabbitMQ"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>)).Endpoint(config => { config.InstanceId = "MassTransit-RabbitMQ"; config.ConfigureConsumeTopology = false; });

        }

        private static void AddReceiveEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
        {
            cfg.ReceiveEndpoint("mass-transit.rabbit-mq.services.animals.order-animal", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<WrapperConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.rabbit-mq.services.animals.make-sound-command", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<WrapperConsumer<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>>(context);
            });
            cfg.ReceiveEndpoint("Person", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<WrapperConsumer<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>>(context);
                e.Consumer<WrapperConsumer<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>>(context);
            });
        }

        private static void EndpointConventionRegistration()
        {
            EndpointConvention.Map<CreatePersonIdentity>(new Uri("queue:Person"));
            EndpointConvention.Map<MakeSoundCommand>(new Uri("queue:mass-transit.rabbit-mq.services.animals.make-sound-command"));
            EndpointConvention.Map<OrderAnimal>(new Uri("queue:mass-transit.rabbit-mq.services.animals.order-animal"));
            EndpointConvention.Map<TalkToPersonCommand>(new Uri("queue:Person"));
        }

        private static void ConfigureNonDefaultEndpoints(
            this IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.AddCustomConsumerEndpoint<WrapperConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(
                context,
                "MassTransit-RabbitMQ",
                endpoint =>
                {
                    endpoint.PrefetchCount = 15;
                    endpoint.Lazy = true;
                    endpoint.Durable = true;
                    endpoint.PurgeOnStartup = true;
                    endpoint.Exclusive = true;
                });
            cfg.AddCustomConsumerEndpoint<WrapperConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(
                context,
                "MassTransit-RabbitMQ",
                endpoint =>
                {
                    endpoint.PrefetchCount = 15;
                    endpoint.Lazy = true;
                    endpoint.Durable = true;
                    endpoint.PurgeOnStartup = true;
                    endpoint.Exclusive = true;
                });
        }

        private static void AddCustomConsumerEndpoint<TConsumer>(
            this IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            string instanceId,
            Action<IRabbitMqReceiveEndpointConfigurator> configuration)
            where TConsumer : class, IConsumer
        {
            cfg.ReceiveEndpoint(
                new ConsumerEndpointDefinition<TConsumer>(new EndpointSettings<IEndpointDefinition<TConsumer>>
                {
                    InstanceId = instanceId
                }),
                KebabCaseEndpointNameFormatter.Instance,
                endpoint =>
                {
                    configuration.Invoke(endpoint);
                    endpoint.ConfigureConsumer<TConsumer>(context);
                });
        }
    }
}