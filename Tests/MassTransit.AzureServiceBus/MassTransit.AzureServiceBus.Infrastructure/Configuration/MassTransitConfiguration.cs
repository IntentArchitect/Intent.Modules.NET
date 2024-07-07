using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;
using MassTransit.AzureServiceBus.Services;
using MassTransit.AzureServiceBus.Services.Animals;
using MassTransit.AzureServiceBus.Services.External;
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

                    cfg.Host(configuration["AzureMessageBus:ConnectionString"]);

                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.ConfigureNonDefaultEndpoints(context);
                    cfg.AddMessageTopologyConfiguration();
                    cfg.AddReceiveEndpoints(context);
                    EndpointConventionRegistration();
                });
            });
        }

        private static void AddMessageTopologyConfiguration(this IServiceBusBusFactoryConfigurator cfg)
        {
            cfg.Message<MessageWithTopologyEvent>(x => x.SetEntityName("custom-topic-name"));
            cfg.Message<OverrideMessageCustomSubscribeEvent>(x => x.SetEntityName("another-overridden-message-topic"));
            cfg.Message<OverrideMessageStandardSubscribeEvent>(x => x.SetEntityName("overridden-message-topic"));
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<ConfigTestMessageEvent>, ConfigTestMessageEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<ConfigTestMessageEvent>, ConfigTestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, OverrideMessageStandardSubscribeEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OverrideMessageStandardSubscribeEvent>, OverrideMessageStandardSubscribeEvent>)).Endpoint(config => config.InstanceId = "MassTransit-AzureServiceBus");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>>(typeof(MediatRConsumerDefinition<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>)).ExcludeFromConfigureEndpoints();
        }

        private static void AddReceiveEndpoints(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.animals.order-animal", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<OrderAnimal>, OrderAnimal>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.animals.make-sound-command", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<MakeSoundCommand>, MakeSoundCommand>>(context);
            });
            cfg.ReceiveEndpoint("Person", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<CreatePersonIdentity>, CreatePersonIdentity>>(context);
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<TalkToPersonCommand>, TalkToPersonCommand>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.command-dto-return", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.command-guid-return", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.command-no-param", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.command-void-return", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.query-guid-return", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.query-no-input-dto-return-collection", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>>(context);
            });
            cfg.ReceiveEndpoint("mass-transit.azure-service-bus.services.request-response.cqrs.query-response-dto-return", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<MediatRConsumer<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>>(context);
            });

        }

        private static void EndpointConventionRegistration()
        {
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.People.TalkToPersonCommand>(new Uri("queue:Person"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.Animals.MakeSoundCommand>(new Uri("queue:mass-transit.azure-service-bus.services.animals.make-sound-command"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.People.CreatePersonIdentity>(new Uri("queue:Person"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.Animals.OrderAnimal>(new Uri("queue:mass-transit.azure-service-bus.services.animals.order-animal"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-dto-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-no-param"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-void-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-no-input-dto-return-collection"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-response-dto-return"));
        }

        private static void ConfigureNonDefaultEndpoints(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.AddConsumerReceiveEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(
                context,
                definition => definition.InstanceId = "MassTransit-AzureServiceBus",
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
            cfg.AddConsumerReceiveEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<ConfigTestMessageEvent>, ConfigTestMessageEvent>>(
                context,
                definition => definition.InstanceId = "MassTransit-AzureServiceBus",
                endpoint =>
                {
                    endpoint.RequiresSession = false;
                    endpoint.DefaultMessageTimeToLive = TimeSpan.Parse("1.1:02:03");
                    endpoint.LockDuration = TimeSpan.Parse("1.1:02:03");
                    endpoint.RequiresDuplicateDetection = true;
                    endpoint.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse("1.1:02:03");
                    endpoint.EnableBatchedOperations = true;
                    endpoint.EnableDeadLetteringOnMessageExpiration = true;
                });
            cfg.AddConsumerReceiveEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>>(
                context,
                definition => definition.Name = "custom-receive-endpoint",
                endpoint =>
                {
                    endpoint.RequiresSession = false;
                    endpoint.RequiresDuplicateDetection = false;
                    endpoint.EnableBatchedOperations = true;
                    endpoint.EnableDeadLetteringOnMessageExpiration = true;
                });
            cfg.AddConsumerReceiveEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>>(
                context,
                definition => definition.Name = "another-receiver-endpoint",
                endpoint =>
                {
                    endpoint.RequiresSession = false;
                    endpoint.RequiresDuplicateDetection = false;
                    endpoint.EnableBatchedOperations = true;
                    endpoint.EnableDeadLetteringOnMessageExpiration = true;
                });
            cfg.SubscriptionEndpoint<StandardMessageCustomSubscribeEvent>("custom-subscription-endpoint", endpoint =>
            {
                endpoint.RequiresSession = false;
                endpoint.EnableBatchedOperations = true;
                endpoint.EnableDeadLetteringOnMessageExpiration = true;
                endpoint.ConfigureConsumer<IntegrationEventConsumer<IIntegrationEventHandler<StandardMessageCustomSubscribeEvent>, StandardMessageCustomSubscribeEvent>>(context);
            });
            cfg.SubscriptionEndpoint("another-subscription-endpoint", "another-overridden-message-topic", endpoint =>
            {
                endpoint.RequiresSession = false;
                endpoint.EnableBatchedOperations = true;
                endpoint.EnableDeadLetteringOnMessageExpiration = true;
                endpoint.ConfigureConsumer<IntegrationEventConsumer<IIntegrationEventHandler<OverrideMessageCustomSubscribeEvent>, OverrideMessageCustomSubscribeEvent>>(context);
            });
            cfg.AddConsumerReceiveEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(
                context,
                definition => definition.InstanceId = "MassTransit-AzureServiceBus",
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
                    endpoint.ConcurrentMessageLimit = 10;
                });
        }

        private static void AddConsumerReceiveEndpoint<TConsumer>(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            Action<EndpointSettings<IEndpointDefinition<TConsumer>>> endpointDefinitionConfig,
            Action<IServiceBusReceiveEndpointConfigurator> receiveEndpointConfig)
            where TConsumer : class, IConsumer
        {
            var settings = new EndpointSettings<IEndpointDefinition<TConsumer>>();
            endpointDefinitionConfig(settings);

            cfg.ReceiveEndpoint(
                new ConsumerEndpointDefinition<TConsumer>(settings),
                KebabCaseEndpointNameFormatter.Instance,
                endpoint =>
                {
                    receiveEndpointConfig.Invoke(endpoint);
                    endpoint.ConfigureConsumer<TConsumer>(context);
                });
        }
    }
}