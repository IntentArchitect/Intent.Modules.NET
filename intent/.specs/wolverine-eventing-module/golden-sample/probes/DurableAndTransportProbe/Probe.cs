using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.AzureServiceBus;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.SqlServer;

// Compile-only gate probe for Gate G0 criterion 4 (citable surface) and ledger row a1.
//
// Nothing here runs. Its entire job is to make the WolverineFx APIs the module's templates will
// emit appear in a COMMITTED, COMPILING file, so /sdd-design can cite them verbatim rather than
// guessing them. The sample applications prove RabbitMQ + Outbox None at runtime; everything the
// sample does not cover is cited from here instead.
//
// The root namespace deliberately does NOT begin with "Wolverine" - see Gate G0 criterion 8.
namespace WolverineEventing.Probes;

public sealed class ProbeDbContext(DbContextOptions<ProbeDbContext> options) : DbContext(options);

public static class DurableOutboxProbe
{
    // Transactional Outbox = Durable, backed by SQL Server.
    public static void SqlServer(WolverineOptions options, string connectionString)
    {
        options.PersistMessagesWithSqlServer(connectionString);
        options.Services.AddDbContextWithWolverineIntegration<ProbeDbContext>(
            o => o.UseSqlServer(connectionString));
        options.Policies.AutoApplyTransactions();
        options.Policies.UseDurableLocalQueues();
        options.Policies.UseDurableOutboxOnAllSendingEndpoints();
        options.Policies.UseDurableInboxOnAllListeners();
    }

    // Transactional Outbox = Durable, backed by PostgreSQL.
    public static void Postgresql(WolverineOptions options, string connectionString)
    {
        options.PersistMessagesWithPostgresql(connectionString);
        options.Policies.AutoApplyTransactions();
        options.Policies.UseDurableOutboxOnAllSendingEndpoints();
        options.Policies.UseDurableInboxOnAllListeners();
    }
}

public static class TransportProbe
{
    // Transport = Azure Service Bus.
    public static void AzureServiceBus(WolverineOptions options, string connectionString)
    {
        options.UseAzureServiceBus(connectionString).AutoProvision();
        options.PublishMessage<ProbeMessage>().ToAzureServiceBusTopic("probe-message");
        options.ListenToAzureServiceBusQueue("probe-message");
    }

    // Transport = Amazon SQS.
    public static void AmazonSqs(WolverineOptions options, string region)
    {
        options.UseAmazonSqsTransport(config => config.ServiceURL = region).AutoProvision();
        options.PublishMessage<ProbeMessage>().ToSqsQueue("probe-message");
        options.ListenToSqsQueue("probe-message");
    }

    // Transport = Local. The module's default, and in-process only.
    public static void Local(WolverineOptions options)
    {
        options.PublishMessage<ProbeMessage>().ToLocalQueue("probe-message");
    }

    // Broker Topology = Externally owned: use the destinations, declare nothing.
    public static void RabbitMqExternallyOwned(WolverineOptions options)
    {
        options.UseRabbitMq().DisableSystemRequestReplyQueueDeclaration();
        options.PublishMessage<ProbeMessage>().ToRabbitExchange("probe-message");
        options.ListenToRabbitQueue("probe-message");
    }
}

public record ProbeMessage(Guid Id);
