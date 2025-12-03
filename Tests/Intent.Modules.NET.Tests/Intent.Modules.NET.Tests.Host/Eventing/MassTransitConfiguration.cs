using System.Reflection;
using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Infrastructure.Core.Eventing;
using Intent.Modules.NET.Tests.Infrastructure.Core.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Host.Eventing
{
    public static class MassTransitConfiguration
    {
        public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            IEnumerable<IModuleInstaller> moduleInstallers)
        {
            services.AddScoped<MassTransitMessageBus>();
            services.AddScoped<IEventBus>(provider => provider.GetRequiredService<MassTransitMessageBus>());

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
                });
                x.AddInMemoryInboxOutbox();
                moduleInstallers.ConfigureIntegrationEventConsumers(x);
            });
            return services;
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
        }
    }
}