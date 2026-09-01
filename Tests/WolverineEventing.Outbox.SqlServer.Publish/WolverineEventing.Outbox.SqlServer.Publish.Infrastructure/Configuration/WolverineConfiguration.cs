using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Persistence;
using Wolverine.SqlServer;
using WolverineEventing.Outbox.SqlServer.Publish.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Outbox.SqlServer.Publish.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureAzureServiceBusTransport(opts, configuration);

            ConfigurePublishing(opts);

            ApplyErrorHandlingPolicy(opts, configuration);

            ApplyTransactionalOutbox(opts, configuration);
        }

        private static void ConfigureAzureServiceBusTransport(WolverineOptions opts, IConfiguration configuration)
        {
            const string section = "Wolverine:AzureServiceBus";
            const string key = "ConnectionString";
            var connectionString = configuration[$"{section}:{key}"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.");
            }

            var transport = opts.UseAzureServiceBus(connectionString);

            transport.AutoProvision();
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToAzureServiceBusTopic("order-created-event");
        }

        private static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
        {
            var delays = ParseDelays(configuration["Wolverine:ErrorHandling:RetryWithCooldown:Delays"] ?? "00:00:01, 00:00:05, 00:00:15");

            if (delays.Length == 0)
            {
                opts.OnException<Exception>().MoveToErrorQueue();
            }
            else
            {
                opts.OnException<Exception>().RetryWithCooldown(delays).Then.MoveToErrorQueue();
            }
        }

        private static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(TimeSpan.Parse).ToArray();
        }

        private static void ApplyTransactionalOutbox(WolverineOptions opts, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            opts.PersistMessagesWithSqlServer(connectionString);
            opts.UseEntityFrameworkCoreTransactions(TransactionMiddlewareMode.Lightweight);
            opts.Policies.AutoApplyTransactions();
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
            opts.Policies.UseDurableInboxOnAllListeners();
        }
    }
}