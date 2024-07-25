using System;
using System.Reflection;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Eventing.Messages;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Configuration
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
                    cfg.UseMessageRetry(r => r.Interval(
                        configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                        configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                    cfg.UseInMemoryOutbox(context);
                });
                x.AddInMemoryInboxOutbox();
            });
        }

        private static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<EnumSampleEvent>, EnumSampleEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<EnumSampleEvent>, EnumSampleEvent>)).Endpoint(config => config.InstanceId = "AdvancedMappingCrud-Repositories-Tests");
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<QuoteCreatedIntegrationEvent>, QuoteCreatedIntegrationEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<QuoteCreatedIntegrationEvent>, QuoteCreatedIntegrationEvent>)).Endpoint(config => config.InstanceId = "AdvancedMappingCrud-Repositories-Tests");
        }
    }
}