using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Models;
using Redis.Om.Repositories.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace Redis.Om.Repositories.Application.EventHandlers
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