using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<NServiceBusMessageBus>();
            registry.Register<MsgNServiceBusEvent, NServiceBusMessageBus>();

            services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
            return services;
        }

        private static EndpointConfiguration ConfigureMainEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            ConfigureCommonSettings(endpointConfiguration, configuration);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(MsgNServiceBusEvent) }.Contains);

            return endpointConfiguration;
        }

        private static RoutingSettings ConfigureCommonSettings(
            EndpointConfiguration endpointConfiguration,
            IConfiguration configuration)
        {
            var rawStoragePath = configuration["NServiceBus:LearningTransport:StorageDirectory"];
            var storageDirectory = rawStoragePath is not null
                ? Environment.ExpandEnvironmentVariables(rawStoragePath)
                : Path.Combine(Path.GetTempPath(), "nservicebus-learning");
            var routing = endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = storageDirectory });

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            return routing;
        }
    }
}