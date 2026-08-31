using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.ErrorHandling;
using WolverineEventing.Transport.AzureServiceBus.Application.Common.Interfaces;
using WolverineEventing.Transport.AzureServiceBus.Application.Orders.CreateOrder;
using WolverineEventing.Transport.AzureServiceBus.Eventing.Messages;
using WolverineEventing.Transport.AzureServiceBus.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.AzureServiceBus.Infrastructure.Configuration
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
            opts.Discovery.IncludeType<CreateOrderCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureAzureServiceBusTransport(opts, configuration);

            ConfigurePublishing(opts);

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        private static void ConfigureAzureServiceBusTransport(WolverineOptions opts, IConfiguration configuration)
        {
            const string section = "Wolverine:AzureServiceBus";
            const string key = "ConnectionString";
            var connectionString = configuration[$"{section}:{key}"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.");
            }

            var transport = opts.UseAzureServiceBus(connectionString);

            transport.AutoProvision();
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToAzureServiceBusTopic("order-created-event");
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