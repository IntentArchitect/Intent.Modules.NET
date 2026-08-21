using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using Wolverine.SqlServer;

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        private const string OrderShippedEventQueue = "wolverine-subscribe-rabbitmq-order-shipped-event";
        private const string ProcessOrderCommandQueue = "process-order-command";

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
                .AutoProvision()
                .BindExchange("order-shipped-event").ToQueue(OrderShippedEventQueue);

            options.ListenToRabbitQueue(OrderShippedEventQueue);
            options.ListenToRabbitQueue(ProcessOrderCommandQueue);

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            options.PersistMessagesWithSqlServer(connectionString, "wolverine");
            options.UseEntityFrameworkCoreTransactions();

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
