using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N_ServiceBus.Persistence.NHibernate.Publish.Application.Common.Eventing;
using N_ServiceBus.Persistence.NHibernate.Publish.Eventing.Messages;
using N_ServiceBus.Persistence.NHibernate.Publish.Infrastructure.Eventing;
using NServiceBus.Persistence;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace N_ServiceBus.Persistence.NHibernate.Publish.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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

            var nhibernateConnectionString = configuration.GetConnectionString("NServiceBus") ?? throw new InvalidOperationException("ConnectionStrings:NServiceBus is not configured");
            var nhibernateConfig = new global::NHibernate.Cfg.Configuration();
            nhibernateConfig.SetProperty(global::NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            nhibernateConfig.SetProperty(global::NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.MicrosoftDataSqlClientDriver");
            nhibernateConfig.SetProperty(global::NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2012Dialect");
            nhibernateConfig.SetProperty(global::NHibernate.Cfg.Environment.ConnectionString, nhibernateConnectionString);
            var nhibPersistence = endpointConfiguration.UsePersistence<NHibernatePersistence>();
            nhibPersistence.UseConfiguration(nhibernateConfig);
            nhibPersistence.EnableTransactionalSession();

            endpointConfiguration.EnableOutbox();
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            endpointConfiguration.Recoverability()
                .Immediate(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5)))
                .Delayed(r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10))));
            endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");

            ConfigureMessageConventions(endpointConfiguration);

            routing.RouteToEndpoint(typeof(TestCommand), configuration["NServiceBus:Routing:Commands:TestCommand"] ?? "N_ServiceBus.Persistence.NHibernate.Subscribe");

            return endpointConfiguration;
        }

        private static void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
        {
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(new[] { typeof(TestEvent) }.Contains);
            conventions.DefiningCommandsAs(new[] { typeof(TestCommand) }.Contains);
        }
    }
}
