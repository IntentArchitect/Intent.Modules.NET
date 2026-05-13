using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityOperationInvokedEventHandler : IIntegrationEventHandler<EntityOperationInvokedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public EntityOperationInvokedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(EntityOperationInvokedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (EntityOperationInvokedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}