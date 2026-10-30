using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.TransactionalSession;
using TestMediator.Nsb.Application.Common.Eventing;
using TestMediator.Nsb.Eventing.Messages;
using TestMediator.Nsb.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace TestMediator.Nsb.Infrastructure.Configuration
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

            var rawStoragePath = configuration["NServiceBus:LearningTransport:StorageDirectory"];

            var storageDirectory = rawStoragePath is not null
                ? Environment.ExpandEnvironmentVariables(rawStoragePath)
                : Path.Combine(Path.GetTempPath(), "nservicebus-learning");
            endpointConfiguration.UseTransport(new LearningTransport { StorageDirectory = storageDirectory, TransportTransactionMode = TransportTransactionMode.ReceiveOnly });

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

            return endpointConfiguration;
        }

        private static void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
        {
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(OrderAcceptedEvent), typeof(OrderPlacedEvent) }.Contains);
        }

        private static void RegisterHandlers(EndpointConfiguration endpointConfiguration)
        {
            RegisterHandler<NServiceBusMessageHandler<OrderPlacedEvent>, OrderPlacedEvent>(endpointConfiguration);
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