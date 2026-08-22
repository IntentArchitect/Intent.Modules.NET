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
            // Extend handler discovery to this Infrastructure assembly, where the Consumers live.
            //
            // Two separate reasons it is needed, and neither is covered elsewhere:
            //   1. WolverineOptions.ApplicationAssembly is the ENTRY assembly - the .Api project -
            //      so Infrastructure is not scanned by default. Verified against 5.39.5: default
            //      discovery yields 0 handler chains here; this call yields 2, one per Consumer.
            //   2. Intent.Application.Wolverine's generated WolverineConfiguration includes only
            //      the Application assembly (typeof(ICommand).Assembly), so installing the CQRS
            //      module does not cover this either.
            //
            // Without it Wolverine logs "found no handlers", listeners receive messages that route
            // nowhere, and no Integration Event Handler is ever invoked.
            options.Discovery.IncludeAssembly(typeof(WolverineEventingConfiguration).Assembly);

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
