using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MassTransit.AzureServiceBus.Infrastructure.Eventing;
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
                });
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>)).ExcludeFromConfigureEndpoints();
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