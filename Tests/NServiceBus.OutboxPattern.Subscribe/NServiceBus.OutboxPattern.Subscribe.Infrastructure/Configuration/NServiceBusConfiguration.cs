using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Subscribe.Infrastructure.Eventing;
using NServiceBus.TransactionalSession;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.NServiceBus.NServiceBusConfiguration", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Infrastructure.Configuration
{
    public static class NServiceBusConfiguration
    {
        public static IHostBuilder AddNServiceBus(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            return hostBuilder.UseNServiceBus(ctx => ConfigureEndpoint(configuration));
        }

        public static IServiceCollection AddNServiceBusConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<NServiceBusMessageBus>();
            services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<NServiceBusMessageBus>());
            return services;
        }

        private static EndpointConfiguration ConfigureEndpoint(IConfiguration configuration)
        {
            var endpointName = configuration["NServiceBus:EndpointName"] ?? throw new InvalidOperationException("NServiceBus:EndpointName is not configured");
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("ConnectionStrings:RabbitMQ is not configured");
            var transportConfig = endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));

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

            return endpointConfiguration;
        }
    }
}