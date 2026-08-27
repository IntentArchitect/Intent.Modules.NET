using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.AzureServiceBus;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
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

// R7.1 - Error Handling Policy call shapes with no citation in the sample or the probe until now.
// Every branch terminates in MoveToErrorQueue() per R7.3; the .Then chain is confirmed against
// Wolverine.ErrorHandling.PolicyExpression / IAdditionalActions at the pinned version.
public static class ErrorHandlingPolicyProbe
{
    // Error Handling Policy = Retry: bounded retry-immediately count, then move-to-error-queue.
    public static void Retry(WolverineOptions options, int attempts)
    {
        options.OnException<Exception>().RetryTimes(attempts).Then.MoveToErrorQueue();
    }

    // Error Handling Policy = Schedule retry: scheduled attempts over an ordered delay sequence,
    // releasing the listener between attempts, then move-to-error-queue. Same shape as the
    // Golden Sample's RetryWithCooldown branch, substituting ScheduleRetry.
    public static void ScheduleRetry(WolverineOptions options, TimeSpan[] delays)
    {
        if (delays.Length == 0)
        {
            options.OnException<Exception>().MoveToErrorQueue();
        }
        else
        {
            options.OnException<Exception>().ScheduleRetry(delays).Then.MoveToErrorQueue();
        }
    }
}

// R2.6 - the fail-fast configuration read for Azure Service Bus and Amazon SQS. Plain
// Microsoft.Extensions.Configuration usage, no WolverineFx-specific surface, included here so
// the call shape that differs from the Golden Sample's silent-default RabbitMQ reader is cited
// alongside the rest of D6's uncited criteria rather than left to be invented in the template.
public static class FailFastConfigurationProbe
{
    // Transport = Azure Service Bus: ConnectionString has no default and must throw when absent.
    public static string AzureServiceBusConnectionString(IConfiguration configuration)
    {
        const string section = "Wolverine:AzureServiceBus";
        const string key = "ConnectionString";
        var value = configuration[$"{section}:{key}"];
        return string.IsNullOrEmpty(value)
            ? throw new InvalidOperationException(
                $"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.")
            : value;
    }

    // Transport = Amazon SQS: Region has no default and must throw when absent.
    public static string AmazonSqsRegion(IConfiguration configuration)
    {
        const string section = "Wolverine:AmazonSqs";
        const string key = "Region";
        var value = configuration[$"{section}:{key}"];
        return string.IsNullOrEmpty(value)
            ? throw new InvalidOperationException(
                $"Configuration key '{key}' in section '{section}' is required when Transport is Amazon SQS.")
            : value;
    }
}

// R12.2 - the Finbuckle-aware Wolverine middleware shape. Nothing in the Golden Sample or the
// probe touches multi-tenancy; this is the one criterion in the design whose feasibility was
// assumed rather than cited. Confirmed against Finbuckle.MultiTenant.Abstractions and Wolverine's
// Before/Finally middleware convention (Wolverine.Middleware.MiddlewarePolicy) at the pinned
// versions - not against a runtime resolution, which only the Wolverine.MultiTenancy Test
// Application (R19) can demonstrate.
public static class TenancyMiddlewareProbe
{
    // Establishes Finbuckle tenant context from the inbound envelope's Tenant Identifier header
    // before the handler runs, using the same resolve-by-identifier shape ITenantResolver already
    // exposes, then restores the prior context afterwards so this is safe under Wolverine's
    // pooled/re-entrant execution. A message with no header (R12.2 criterion 4) leaves the prior
    // context untouched rather than establishing an empty one or rejecting the message.
    public static IMultiTenantContext? Before(
        Envelope envelope,
        ITenantResolver tenantResolver,
        IMultiTenantContextAccessor contextAccessor,
        IMultiTenantContextSetter contextSetter)
    {
        var previous = contextAccessor.MultiTenantContext;

        if (!envelope.Headers.TryGetValue("tenant-id", out var tenantId) || string.IsNullOrEmpty(tenantId))
        {
            return previous;
        }

        contextSetter.MultiTenantContext = tenantResolver.ResolveAsync(envelope).GetAwaiter().GetResult();
        return previous;
    }

    public static Task FinallyAsync(IMultiTenantContext? previous, IMultiTenantContextSetter contextSetter)
    {
        contextSetter.MultiTenantContext = previous!;
        return Task.CompletedTask;
    }
}

public record ProbeMessage(Guid Id);
