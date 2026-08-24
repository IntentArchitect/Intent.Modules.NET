using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware;

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Eventing
{
    /// <summary>
    /// Golden sample for the Wolverine eventing module's host contribution. Transport = RabbitMQ,
    /// Transactional Outbox = None, Broker Topology = Auto-provision, Error Handling = Retry with
    /// cooldown.
    /// </summary>
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureRabbitMq(WolverineOptions options, IConfiguration configuration)
        {
            ConfigureHandlerDiscovery(options);
            ConfigureTransport(options, configuration);
            ConfigurePublishing(options);
            ConfigureMessageBusFlush(options);
            ConfigureErrorHandling(options, configuration);
        }

        /// <summary>
        /// Deterministic handler registration. This is the shape Intent.Wolverine.Common should own,
        /// because DisableConventionalDiscovery is global and no single module can safely call it.
        /// </summary>
        /// <remarks>
        /// Convention-based discovery matches any type whose name ends in Handler or Consumer. That
        /// is too broad once more than one module contributes handlers: the Application layer's
        /// IIntegrationEventHandler implementations match it, and so would a generated Consumer, so
        /// Wolverine finds two handlers for one message and its runtime codegen emits the same local
        /// variable twice (CS0128) - codegen fails, no handler runs, and messages are dropped in
        /// silence. Registering explicitly removes that whole class of accident, and it is what
        /// makes per-message provider filtering possible when several providers are installed.
        ///
        /// Note the CQRS handlers are registered here too. Intent.Application.Wolverine currently
        /// only emits Discovery.IncludeAssembly(typeof(ICommand).Assembly), relying on the
        /// convention; disabling that globally means its handlers need explicit entries as well.
        /// </remarks>
        private static void ConfigureHandlerDiscovery(WolverineOptions options)
        {
            options.Discovery.DisableConventionalDiscovery();
            options.Discovery.IncludeType<ShipOrderCommandHandler>();
            options.Discovery.IncludeType<RequestOrderProcessingCommandHandler>();
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
                .AutoProvision();
        }

        private static void ConfigurePublishing(WolverineOptions options)
        {
            // Integration Event to an exchange, fan-out. Destination is the message type kebab-cased.
            options.PublishMessage<OrderShippedEvent>().ToRabbitExchange("order-shipped-event");

            // Integration Command to a queue, point-to-point.
            options.PublishMessage<ProcessOrderCommand>().ToRabbitQueue("process-order-command");
        }

        /// <summary>
        /// Registers the flush middleware. Nothing else provides it: under MediatR the flush came
        /// from Intent.Application.MediatR.Behaviours, and Intent.Application.Wolverine ships no
        /// equivalent, so without this Publish/Send queue a message that never leaves the process
        /// and nothing throws.
        /// </summary>
        private static void ConfigureMessageBusFlush(WolverineOptions options)
        {
            options.Policies.AddMiddleware<MessageBusPublishMiddleware>(
                chain => typeof(ICommand).IsAssignableFrom(chain.MessageType));
            options.Services.AddTransient<MessageBusPublishMiddleware>();
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
            // rather than retrying forever. RetryWithCooldown requires at least one delay.
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
