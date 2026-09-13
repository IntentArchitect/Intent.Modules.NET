using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;
using WolverineEventing.ErrorPolicy.None.Application.IntegrationEvents.EventHandlers;
using WolverineEventing.ErrorPolicy.None.Eventing.Messages;
using WolverineEventing.ErrorPolicy.None.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.None.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        private static readonly HashSet<Type> IntegrationMessageTypes = new()
        {
            typeof(OrderCreatedEvent)
        };
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigurePublishing(opts);

            ConfigureListeners(opts);

            ApplyErrorHandlingPolicy(opts, configuration);

            ApplyIntegrationEventPolicy(opts);
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
            opts.OnException<Exception>().MoveToErrorQueue();
        }

        private static void ApplyIntegrationEventPolicy(WolverineOptions opts)
        {
            opts.Policies.AddMiddleware<WolverineIntegrationEventMiddleware>(IsIntegrationMessage);
        }

        private static bool IsIntegrationMessage(HandlerChain chain)
        {
            return IntegrationMessageTypes.Contains(chain.MessageType);
        }
    }
}