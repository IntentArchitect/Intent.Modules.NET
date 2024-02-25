using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RetryPolicy.Exponential.Application.Common.Eventing;
using MassTransit.RetryPolicy.Exponential.Infrastructure.Eventing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace MassTransit.RetryPolicy.Exponential.Infrastructure.Configuration
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

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Exponential(
                        configuration.GetValue<int?>("MassTransit:RetryExponential:RetryLimit") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryExponential:MinInterval") ?? TimeSpan.FromSeconds(5),
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryExponential:MaxInterval") ?? TimeSpan.FromMinutes(30),
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryExponential:IntervalDelta") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox(context);
                });
                x.AddInMemoryInboxOutbox();
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }
    }
}