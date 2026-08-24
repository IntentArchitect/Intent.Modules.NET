using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers;

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    /// <summary>
    /// Golden sample for the Wolverine eventing module's subscribing host contribution. Transport =
    /// RabbitMQ, Transactional Outbox = None, Broker Topology = Auto-provision, Error Handling =
    /// Retry with cooldown.
    /// </summary>
    public static class WolverineEventingConfiguration
    {
        // An Integration Event fans out, so each subscriber needs its OWN queue bound to the shared
        // exchange - hence the application-name prefix. An Integration Command is point-to-point, so
        // its queue name is the message name only and is shared by every sender.
        private const string OrderShippedEventQueue = "wolverine-eventing-subscribe-rabbitmq-order-shipped-event";
        private const string OrderShippedEventExchange = "order-shipped-event";
        private const string ProcessOrderCommandQueue = "process-order-command";

        public static void ConfigureRabbitMq(WolverineOptions options, IConfiguration configuration)
        {
            ConfigureHandlerDiscovery(options);
            ConfigureTransport(options, configuration);
            ConfigureListeners(options);
            ConfigureErrorHandling(options, configuration);
        }

        /// <summary>
        /// Deterministic handler registration - the shape Intent.Wolverine.Common should own.
        /// </summary>
        /// <remarks>
        /// The hand-written IIntegrationEventHandler implementations ARE the Wolverine handlers:
        /// each is named <c>&lt;Message&gt;Handler</c> and exposes <c>HandleAsync(TMessage,
        /// CancellationToken)</c>, which is exactly what Wolverine invokes. No generated Consumer
        /// class sits in between, and adding one would be actively harmful - Wolverine would then
        /// find two handlers for the same message and its runtime codegen would emit a duplicate
        /// local variable (CS0128), failing silently with messages dropped.
        ///
        /// Registering by type rather than by convention is also what lets the module generate
        /// entries only for messages designated to Wolverine when several providers are installed.
        /// </remarks>
        private static void ConfigureHandlerDiscovery(WolverineOptions options)
        {
            options.Discovery.DisableConventionalDiscovery();
            options.Discovery.IncludeType<OrderShippedEventHandler>();
            options.Discovery.IncludeType<ProcessOrderCommandHandler>();
        }

        private static void ConfigureTransport(WolverineOptions options, IConfiguration configuration)
        {
            var section = configuration.GetSection("Wolverine:RabbitMq");
            var host = section["Host"] ?? "localhost";
            var port = int.Parse(section["Port"] ?? "5672");
            var virtualHost = section["VirtualHost"] ?? "/";
            var username = section["Username"] ?? "guest";
            var password = section["Password"] ?? "guest";

            options.UseRabbitMq(rabbit =>
                {
                    rabbit.HostName = host;
                    rabbit.Port = port;
                    rabbit.VirtualHost = virtualHost;
                    rabbit.UserName = username;
                    rabbit.Password = password;
                })
                .AutoProvision()
                .BindExchange(OrderShippedEventExchange).ToQueue(OrderShippedEventQueue);
        }

        private static void ConfigureListeners(WolverineOptions options)
        {
            options.ListenToRabbitQueue(OrderShippedEventQueue);
            options.ListenToRabbitQueue(ProcessOrderCommandQueue);
        }

        private static void ConfigureErrorHandling(WolverineOptions options, IConfiguration configuration)
        {
            var section = configuration.GetSection("Wolverine:ErrorHandling:RetryWithCooldown:Delays");
            var delays = (section.Exists()
                    ? section.Get<string[]>() ?? Array.Empty<string>()
                    : new[] { "00:00:01", "00:00:05", "00:00:15" })
                .Select(TimeSpan.Parse)
                .ToArray();

            // An empty Delays list degrades to no retry: the first failure goes to the Error Queue
            // rather than retrying forever.
            if (delays.Length == 0)
            {
                options.OnException<Exception>().MoveToErrorQueue();
            }
            else
            {
                options.OnException<Exception>().RetryWithCooldown(delays).Then.MoveToErrorQueue();
            }
        }
    }
}
