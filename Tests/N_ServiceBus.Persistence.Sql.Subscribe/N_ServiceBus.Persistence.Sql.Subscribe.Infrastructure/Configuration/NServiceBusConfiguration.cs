using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.Persistence.Sql.Publish.Eventing.Messages;
using N_ServiceBus.Persistence.Sql.Subscribe.Application.Common.Eventing;
using N_ServiceBus.Persistence.Sql.Subscribe.Eventing.Messages;
using N_ServiceBus.Persistence.Sql.Subscribe.Infrastructure.Eventing;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace N_ServiceBus.Persistence.Sql.Subscribe.Infrastructure.Configuration
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

            var licensePath = configuration["NServiceBus:LicensePath"];

            if (licensePath is not null)
            {
                endpointConfiguration.LicensePath(licensePath);
            }

            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("ConnectionStrings:RabbitMQ is not configured");
            var routing = endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));

            var persistenceConnectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured");
            var sqlPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            sqlPersistence.SqlDialect<SqlDialect.MsSqlServer>();
            sqlPersistence.ConnectionBuilder(connectionBuilder: () => new SqlConnection(persistenceConnectionString));
            sqlPersistence.EnableTransactionalSession();
            endpointConfiguration.EnableOutbox();

            if (configuration.GetValue<bool>("NServiceBus:EnableInstallers"))
            {
                endpointConfiguration.EnableInstallers();
            }
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();

            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            ConfigureMessageConventions(endpointConfiguration);
            RegisterHandlers(endpointConfiguration);

            routing.RouteToEndpoint(typeof(TestCommand), endpointName);

            return endpointConfiguration;
        }

        private static void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
        {
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(AnotherTestMessageEvent), typeof(TestEvent) }.Contains);
            conventions.DefiningCommandsAs(new[] { typeof(TestCommand) }.Contains);
        }

        private static void RegisterHandlers(EndpointConfiguration endpointConfiguration)
        {
            RegisterHandler<NServiceBusMessageHandler<AnotherTestMessageEvent>, AnotherTestMessageEvent>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<TestEvent>, TestEvent>(endpointConfiguration);
            RegisterHandler<NServiceBusMessageHandler<TestCommand>, TestCommand>(endpointConfiguration);
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