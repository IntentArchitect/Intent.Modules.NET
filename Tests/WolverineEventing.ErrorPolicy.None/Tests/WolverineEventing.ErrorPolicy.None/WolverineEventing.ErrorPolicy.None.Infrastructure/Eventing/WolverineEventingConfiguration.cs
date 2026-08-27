using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using WolverineEventing.ErrorPolicy.None.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineEventingConfiguration", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.None.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureLocal(WolverineOptions opts, IConfiguration configuration)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToLocalQueue("order-created-event");

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        public static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
        {
            opts.OnException<Exception>().MoveToErrorQueue();
        }

        public static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
.Select(TimeSpan.Parse)
.ToArray();
        }
    }
}