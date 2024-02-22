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

                    cfg.Host(configuration["AzureMessageBus:ConnectionString"]);

                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.ConfigureNonDefaultEndpoints(context);
                    cfg.AddReceiveEndpoints(context);
                    EndpointConventionRegistration();
                });
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>)).ExcludeFromConfigureEndpoints();
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
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-dto-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandNoParam>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-no-param"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandVoidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.command-void-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryGuidReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-guid-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryNoInputDtoReturnCollection>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-no-input-dto-return-collection"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDtoReturn>(new Uri("queue:mass-transit.azure-service-bus.services.request-response.cqrs.query-response-dto-return"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.People.TalkToPersonCommand>(new Uri("queue:Person"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.Animals.MakeSoundCommand>(new Uri("queue:mass-transit.azure-service-bus.services.animals.make-sound-command"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.People.CreatePersonIdentity>(new Uri("queue:Person"));
            EndpointConvention.Map<MassTransit.AzureServiceBus.Services.Animals.OrderAnimal>(new Uri("queue:mass-transit.azure-service-bus.services.animals.order-animal"));
        }

        private static void ConfigureNonDefaultEndpoints(
            this IServiceBusBusFactoryConfigurator cfg,
            IBusRegistrationContext context)
        {
            cfg.AddCustomConsumerEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(
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
            cfg.AddCustomConsumerEndpoint<IntegrationEventConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(
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