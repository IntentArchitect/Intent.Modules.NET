using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.FailOrder;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureCqrs(opts);

            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureCqrs(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Discovery.IncludeType<FailOrderCommandHandler>();
            opts.Discovery.IncludeType<RequestOrderProcessingCommandHandler>();
            opts.Discovery.IncludeType<ShipOrderCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            var transport = ConfigureRabbitMqTransport(opts, configuration);

            ConfigurePublishing(opts);

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        private static RabbitMqTransportExpression ConfigureRabbitMqTransport(
            WolverineOptions opts,
            IConfiguration configuration)
        {
            var section = configuration.GetSection("Wolverine:RabbitMq");
            var host = section["Host"] ?? "localhost";
            var port = int.Parse(section["Port"] ?? "5672");
            var virtualHost = section["VirtualHost"] ?? "/";
            var username = section["Username"] ?? "guest";
            var password = section["Password"] ?? "guest";

            var transport = opts.UseRabbitMq(rabbit =>
            {
                rabbit.HostName = host;
                rabbit.Port = port;
                rabbit.VirtualHost = virtualHost;
                rabbit.UserName = username;
                rabbit.Password = password;
            });

            transport.AutoProvision();

            return transport;
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<FailingOrderEvent>().ToRabbitExchange("failing-order-event");
            opts.PublishMessage<OrderShippedEvent>().ToRabbitExchange("order-shipped-event");
            opts.PublishMessage<ProcessOrderCommand>().ToRabbitQueue("process-order-command");
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
    }
}