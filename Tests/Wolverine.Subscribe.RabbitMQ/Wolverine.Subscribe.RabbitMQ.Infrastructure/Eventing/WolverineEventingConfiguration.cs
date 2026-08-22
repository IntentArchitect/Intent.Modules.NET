using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        private const string OrderShippedEventQueue = "wolverine-subscribe-rabbitmq-order-shipped-event";
        private const string ProcessOrderCommandQueue = "process-order-command";

        public static void ConfigureRabbitMq(WolverineOptions options, IConfiguration configuration)
        {
            // C12 / assumption a2. Wolverine discovers a concrete <Message>Consumer by naming
            // convention, but ONLY in assemblies it scans, and WolverineOptions.ApplicationAssembly
            // is the entry assembly - the .Api project here. The Consumers live in this
            // .Infrastructure assembly, so without this call Wolverine logs "found no handlers",
            // every listener receives messages that route nowhere, and no Integration Event Handler
            // is ever invoked. Verified against WolverineFx 5.39.5: default discovery yields 0
            // handler chains, IncludeAssembly yields 2 (one per Consumer).
            options.Discovery.IncludeAssembly(typeof(OrderShippedEventConsumer).Assembly);

            var rabbitMqSection = configuration.GetSection("Wolverine:RabbitMq");
            var host = rabbitMqSection["Host"] ?? "localhost";
            var port = int.Parse(rabbitMqSection["Port"] ?? "5672");
            var virtualHost = rabbitMqSection["VirtualHost"] ?? "/";
            var username = rabbitMqSection["Username"] ?? "guest";
            var password = rabbitMqSection["Password"] ?? "guest";

            options.UseRabbitMq(rabbit =>
            {
                rabbit.HostName = host;
                rabbit.Port = port;
                rabbit.VirtualHost = virtualHost;
                rabbit.UserName = username;
                rabbit.Password = password;
            })
                .AutoProvision()
                .BindExchange("order-shipped-event").ToQueue(OrderShippedEventQueue);

            options.ListenToRabbitQueue(OrderShippedEventQueue);
            options.ListenToRabbitQueue(ProcessOrderCommandQueue);

            // Transactional Outbox = None, the module's default. No message store is configured at
            // all, so Wolverine uses buffered (non-durable) endpoints and needs no database. With
            // no inbox, R6.5's at-most-once guarantee does not apply in this configuration.
            ConfigureErrorHandling(options, configuration);
        }

        private static void ConfigureErrorHandling(WolverineOptions options, IConfiguration configuration)
        {
            var section = configuration.GetSection("Wolverine:ErrorHandling:RetryWithCooldown:Delays");

            var delays = (section.Exists()
                ? section.Get<string[]>() ?? Array.Empty<string>()
                : new[] { "00:00:01", "00:00:05", "00:00:15" })
                .Select(TimeSpan.Parse)
                .ToArray();

            // R7.5: an empty Delays list degrades to no retry - the first failure goes straight to
            // the Error Queue rather than retrying indefinitely (RetryWithCooldown requires at least
            // one delay).
            if (delays.Length == 0)
            {
                options.OnException<Exception>()
                    .MoveToErrorQueue();
            }
            else
            {
                options.OnException<Exception>()
                    .RetryWithCooldown(delays)
                    .Then.MoveToErrorQueue();
            }
        }
    }
}
