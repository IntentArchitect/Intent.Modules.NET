using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Eventing.Messages;
using MassTransit.RabbitMQ.Infrastructure.Eventing;
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
                });
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<TestMessageEvent>, TestMessageEvent>)).ExcludeFromConfigureEndpoints();
            cfg.AddConsumer<WrapperConsumer<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>>(typeof(WrapperConsumerDefinition<IIntegrationEventHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>)).ExcludeFromConfigureEndpoints();

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