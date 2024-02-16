using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;
using MassTransit.AzureServiceBus.Services;
using MassTransit.AzureServiceBus.Services.Animals;
using MassTransit.AzureServiceBus.Services.People;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Configuration
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

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.Host(configuration["AzureMessageBus:ConnectionString"]);
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
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>)).Endpoint(config => { config.InstanceId = "MassTransit-AzureServiceBus"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>)).Endpoint(config => { config.InstanceId = "MassTransit-AzureServiceBus"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>)).Endpoint(config => { config.InstanceId = "MassTransit-AzureServiceBus"; config.ConfigureConsumeTopology = false; });
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>)).Endpoint(config => { config.InstanceId = "MassTransit-AzureServiceBus"; config.ConfigureConsumeTopology = false; });
        }

        private static void AddReceiveEndpoints(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.animals.order-animal", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<WrapperConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.animals.make-sound-command", e =>
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
            EndpointConvention.Map<TalkToPersonCommand>(new Uri("queue:Person"));
            EndpointConvention.Map<MakeSoundCommand>(new Uri("queue:mass-transit.azure-service-bus.services.animals.make-sound-command"));
            EndpointConvention.Map<CreatePersonIdentity>(new Uri("queue:Person"));
            EndpointConvention.Map<OrderAnimal>(new Uri("queue:mass-transit.azure-service-bus.services.animals.order-animal"));
        }

        private static void ConfigureNonDefaultEndpoints(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.AddCustomConsumerEndpoint<WrapperConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(
                context,
                "MassTransit-AzureServiceBus",
                endpoint =>
                {
                    endpoint.PrefetchCount = 15;
                    endpoint.RequiresSession = false;
                    endpoint.DefaultMessageTimeToLive = TimeSpan.Parse("00:15:00");
                    endpoint.RequiresDuplicateDetection = true;
                    endpoint.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse("00:10:00");
                    endpoint.EnableBatchedOperations = true;
                    endpoint.EnableDeadLetteringOnMessageExpiration = true;
                    endpoint.MaxSizeInMegabytes = 2048;
                });
            cfg.AddCustomConsumerEndpoint<WrapperConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(
                context,
                "MassTransit-AzureServiceBus",
                endpoint =>
                {
                    endpoint.PrefetchCount = 15;
                    endpoint.RequiresSession = false;
                    endpoint.DefaultMessageTimeToLive = TimeSpan.Parse("00:15:00");
                    endpoint.RequiresDuplicateDetection = true;
                    endpoint.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse("00:10:00");
                    endpoint.EnableBatchedOperations = true;
                    endpoint.EnableDeadLetteringOnMessageExpiration = true;
                    endpoint.MaxSizeInMegabytes = 2048;
                });
        }

        private static void AddCustomConsumerEndpoint<TConsumer>(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            string instanceId,
            Action<IServiceBusReceiveEndpointConfigurator> configuration)
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