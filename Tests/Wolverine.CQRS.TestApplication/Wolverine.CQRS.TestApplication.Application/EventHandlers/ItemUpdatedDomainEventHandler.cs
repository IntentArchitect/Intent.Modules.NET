using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ItemUpdatedDomainEventHandler
    {
        [IntentManaged(Mode.Merge)]
        public ItemUpdatedDomainEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ItemUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ItemUpdatedDomainEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}