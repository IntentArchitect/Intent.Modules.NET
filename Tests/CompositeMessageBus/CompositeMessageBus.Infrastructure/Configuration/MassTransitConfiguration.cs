using System.Reflection;
using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<MassTransitMessageBus>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox(context);
                    cfg.UseDelayedMessageScheduler();
                });
                x.AddInMemoryInboxOutbox();
                x.AddDelayedMessageScheduler();
            });
            registry.Register<MsgMassTEvent, MassTransitMessageBus>();
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }
    }
}