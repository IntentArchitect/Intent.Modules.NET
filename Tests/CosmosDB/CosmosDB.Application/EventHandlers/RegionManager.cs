using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Models;
using CosmosDB.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace CosmosDB.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RegionManager : INotificationHandler<DomainEventNotification<RegionChangedDomainEvent>>
    {

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(DomainEventNotification<RegionChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (RegionManager) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}