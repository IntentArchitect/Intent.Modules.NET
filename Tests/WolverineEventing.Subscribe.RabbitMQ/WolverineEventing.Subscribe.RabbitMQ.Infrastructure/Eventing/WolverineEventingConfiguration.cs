using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers;

[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineEventingConfiguration", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {

        public static void ConfigureRabbitMq(WolverineOptions opts, IConfiguration configuration)
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

            transport.BindExchange("order-shipped-event").ToQueue("wolverine-eventing.subscribe.rabbit-mq-order-shipped-event");

            opts.ListenToRabbitQueue("wolverine-eventing.subscribe.rabbit-mq-order-shipped-event");

            transport.BindExchange("failing-order-event").ToQueue("wolverine-eventing.subscribe.rabbit-mq-failing-order-event");

            opts.ListenToRabbitQueue("wolverine-eventing.subscribe.rabbit-mq-failing-order-event");

            opts.ListenToRabbitQueue("process-order-command");

            opts.Discovery.IncludeType<FailingOrderEventHandler>();

            opts.Discovery.IncludeType<OrderShippedEventHandler>();

            opts.Discovery.IncludeType<ProcessOrderCommandHandler>();

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        public static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
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

        public static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
.Select(TimeSpan.Parse)
.ToArray();
        }
    }
}
