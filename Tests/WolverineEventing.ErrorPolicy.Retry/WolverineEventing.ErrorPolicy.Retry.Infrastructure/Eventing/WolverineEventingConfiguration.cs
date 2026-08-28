using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using WolverineEventing.ErrorPolicy.Retry.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.ErrorPolicy.Retry.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineEventingConfiguration", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.Retry.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureLocal(WolverineOptions opts, IConfiguration configuration)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToLocalQueue("order-created-event");

            opts.Discovery.IncludeType<OrderCreatedEventHandler>();

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        public static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
        {
            var attempts = int.Parse(configuration["Wolverine:ErrorHandling:Retry:Attempts"] ?? "3");

            opts.OnException<Exception>().RetryTimes(attempts).Then.MoveToErrorQueue();
        }

        public static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
.Select(TimeSpan.Parse)
.ToArray();
        }
    }
}