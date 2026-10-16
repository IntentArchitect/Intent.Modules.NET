using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using TestMediator.WolverineEventing.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers;
using TestMediator.WolverineEventing.Eventing.Messages;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Persistence;
using Wolverine.SqlServer;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace TestMediator.WolverineEventing.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigurePublishing(opts);

            ConfigureListeners(opts);

            ApplyErrorHandlingPolicy(opts, configuration);

            ApplyTransactionalOutbox(opts, configuration);
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<OrderAcceptedEvent>().ToLocalQueue("order-accepted-event");
        }

        private static void ConfigureListeners(WolverineOptions opts)
        {
            opts.Discovery.IncludeType<OrderPlacedEventHandler>();
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