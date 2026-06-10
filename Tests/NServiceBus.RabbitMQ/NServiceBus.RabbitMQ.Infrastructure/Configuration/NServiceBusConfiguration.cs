using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.RabbitMQ.Application.Common.Eventing;
using NServiceBus.RabbitMQ.Eventing.Messages;
using NServiceBus.RabbitMQ.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());

            services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
            return services;
        }

        private static EndpointConfiguration ConfigureMainEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            ConfigureCommonSettings(endpointConfiguration, configuration);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(TestMessageEvent) }.Contains);
            conventions.DefiningCommandsAs(new[] { typeof(OrderAnimal), typeof(CreatePersonIdentity), typeof(MakeSoundCommand), typeof(TalkToPersonCommand) }.Contains);

            return endpointConfiguration;
        }

        private static RoutingSettings ConfigureCommonSettings(
            EndpointConfiguration endpointConfiguration,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("ConnectionStrings:RabbitMQ is not configured");
            var routing = endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));

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