using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Models;
using CosmosDB.EntityInterfaces.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.EventHandlers
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
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}