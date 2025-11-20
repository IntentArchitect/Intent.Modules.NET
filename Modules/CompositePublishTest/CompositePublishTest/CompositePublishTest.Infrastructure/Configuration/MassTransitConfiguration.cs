using System;
using CompositePublishTest.Eventing.Messages;
using CompositePublishTest.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for MassTransit message broker provider.
    /// Handles both publishing (via CMB provider) and consuming (for full MassTransit features).
    /// </summary>
    public static class MassTransitConfiguration
    {
        /// <summary>
        /// Configures MassTransit infrastructure including the provider for CMB integration.
        /// </summary>
        public static IServiceCollection ConfigureMassTransit(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<MassTransitMessageBus>();
            
            registry.Register<ClientCreatedEvent, MassTransitMessageBus>();

            // Setup full MassTransit with RabbitMQ transport and consumers
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
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
            });

            return services;
        }
    }
}

