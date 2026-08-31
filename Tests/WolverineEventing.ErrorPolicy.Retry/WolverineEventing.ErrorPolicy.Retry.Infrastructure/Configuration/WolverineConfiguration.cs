using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using WolverineEventing.ErrorPolicy.Retry.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.ErrorPolicy.Retry.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.Retry.Infrastructure.Configuration
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
            var attempts = int.Parse(configuration["Wolverine:ErrorHandling:Retry:Attempts"] ?? "3");

            opts.OnException<Exception>().RetryTimes(attempts).Then.MoveToErrorQueue();
        }
    }
}