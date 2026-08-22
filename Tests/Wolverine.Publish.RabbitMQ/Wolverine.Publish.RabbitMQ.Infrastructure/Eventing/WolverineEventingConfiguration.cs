using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

namespace Wolverine.Publish.RabbitMQ.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureRabbitMq(WolverineOptions options, IConfiguration configuration)
        {
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
                .AutoProvision();

            options.PublishMessage<Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent>()
                .ToRabbitExchange("order-shipped-event");

            options.PublishMessage<Wolverine.Publish.RabbitMQ.Eventing.Messages.ProcessOrderCommand>()
                .ToRabbitQueue("process-order-command");

            // Transactional Outbox = None, the module's default. No message store is configured at
            // all, so Wolverine uses buffered (non-durable) endpoints and needs no database. This
            // is the absence of configuration rather than a mode setting - and note that
            // DurabilityMode.MediatorOnly is NOT the way to express it: that disables external
            // messaging entirely and makes PublishAsync throw.
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
