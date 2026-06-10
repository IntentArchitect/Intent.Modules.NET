using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.OutboxPattern.Publish.Eventing.Messages;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Subscribe.Eventing.Messages;
using NServiceBus.OutboxPattern.Subscribe.Infrastructure.Eventing;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());

            services.AddNServiceBusEndpoint(ConfigureEndpointForTestCommand(configuration));
            services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
            return services;
        }

        private static EndpointConfiguration ConfigureMainEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            ConfigureCommonSettings(endpointConfiguration, configuration);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(AnotherTestMessageEvent), typeof(TestEvent) }.Contains);
            RegisterHandler<NServiceBusMessageHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<TestEvent>, TestEvent>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureEndpointForTestCommand(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:Routing:Commands:TestCommand"] ?? "test-command";
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            ConfigureCommonSettings(endpointConfiguration, configuration);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(new[] { typeof(TestCommand) }.Contains);
            RegisterHandler<NServiceBusMessageHandler<TestCommand>, TestCommand>(endpointConfiguration);

            return endpointConfiguration;
        }

        private static RoutingSettings ConfigureCommonSettings(
            EndpointConfiguration endpointConfiguration,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("ConnectionStrings:RabbitMQ is not configured");
            var routing = endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));

            var persistenceConnectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured");
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(connectionBuilder: () => new SqlConnection(persistenceConnectionString));
            persistence.EnableTransactionalSession();
            endpointConfiguration.EnableOutbox();

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            return routing;
        }

        private static void RegisterHandler<THandler, TMessage>(EndpointConfiguration endpointConfiguration)
            where THandler : class, IHandleMessages<TMessage>
            where TMessage : class
        {
            var settings = NServiceBus.Configuration.AdvancedExtensibility.AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);
            var messageHandlerRegistry = settings.GetOrCreate<NServiceBus.Unicast.MessageHandlerRegistry>();
            var messageMetadataRegistry = settings.GetOrCreate<NServiceBus.Unicast.Messages.MessageMetadataRegistry>();
            messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();
            messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());
        }
    }
}