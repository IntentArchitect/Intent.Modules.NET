using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.ErrorHandling;
using WolverineEventing.Transport.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineEventingConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.AzureServiceBus.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureAzureServiceBus(WolverineOptions opts, IConfiguration configuration)
        {
            const string section = "Wolverine:AzureServiceBus";
            const string key = "ConnectionString";
            var connectionString = configuration[$"{section}:{key}"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new System.InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.");
            }

            var transport = opts.UseAzureServiceBus(connectionString);

            transport.AutoProvision();

            opts.PublishMessage<OrderCreatedEvent>().ToAzureServiceBusTopic("order-created-event");

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