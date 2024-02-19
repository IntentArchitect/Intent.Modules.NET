using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Application.Common.Models;
using CosmosDB.PrivateSetters.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RegionManager : INotificationHandler<DomainEventNotification<RegionChangedDomainEvent>>
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DomainEventNotification<RegionChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}