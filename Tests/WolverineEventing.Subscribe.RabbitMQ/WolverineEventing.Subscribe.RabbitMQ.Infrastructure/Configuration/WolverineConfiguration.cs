using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Interfaces;
using WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Configuration
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
            ApplicationHandlerPolicy.Apply(opts);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            var transport = ConfigureRabbitMqTransport(opts, configuration);

            ConfigureListeners(opts, transport);

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

        private static void ConfigureListeners(WolverineOptions opts, RabbitMqTransportExpression transport)
        {
            transport.BindExchange("order-shipped-event").ToQueue("wolverine-eventing.subscribe.rabbit-mq-order-shipped-event");

            opts.ListenToRabbitQueue("wolverine-eventing.subscribe.rabbit-mq-order-shipped-event");

            transport.BindExchange("failing-order-event").ToQueue("wolverine-eventing.subscribe.rabbit-mq-failing-order-event");

            opts.ListenToRabbitQueue("wolverine-eventing.subscribe.rabbit-mq-failing-order-event");

            opts.ListenToRabbitQueue("process-order-command");

            opts.Discovery.IncludeType<FailingOrderEventHandler>();

            opts.Discovery.IncludeType<OrderShippedEventHandler>();

            opts.Discovery.IncludeType<ProcessOrderCommandHandler>();
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