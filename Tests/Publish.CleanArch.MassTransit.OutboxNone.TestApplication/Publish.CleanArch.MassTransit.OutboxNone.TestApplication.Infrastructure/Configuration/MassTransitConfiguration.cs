using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
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
                    cfg.AddMessageTopologyConfiguration();
                });
            });
        }

        private static void AddMessageTopologyConfiguration(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Message<BasketCreatedEvent>(x => x.SetEntityName("basket-created-topic-rename"));
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {

        }
    }
}