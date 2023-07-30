using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:Retry:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:Retry:Interval") ?? TimeSpan.FromSeconds(30)));
                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox();
                });
                x.AddInMemoryInboxOutbox();
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }
    }
}