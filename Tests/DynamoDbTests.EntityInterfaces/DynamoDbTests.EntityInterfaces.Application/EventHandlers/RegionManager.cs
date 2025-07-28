using DynamoDbTests.EntityInterfaces.Application.Common.Models;
using DynamoDbTests.EntityInterfaces.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RegionManager : INotificationHandler<DomainEventNotification<RegionChangedDomainEvent>>
    {
        public RegionManager()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DomainEventNotification<RegionChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (RegionManager) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}