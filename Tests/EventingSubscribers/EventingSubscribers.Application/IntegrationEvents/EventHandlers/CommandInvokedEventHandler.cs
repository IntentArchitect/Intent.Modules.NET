using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandInvokedEventHandler : IIntegrationEventHandler<CommandInvokedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public CommandInvokedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CommandInvokedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (CommandInvokedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}