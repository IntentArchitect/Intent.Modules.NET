using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.RetryWithCooldown.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigurePublishing(opts);

            ConfigureListeners(opts);

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToLocalQueue("order-created-event");
        }

        private static void ConfigureListeners(WolverineOptions opts)
        {
            opts.Discovery.IncludeType<OrderCreatedEventHandler>();
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