using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Models;
using RichDomain.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace RichDomain.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PersonUpdatedEventHandler : INotificationHandler<DomainEventNotification<PersonUpdatedEvent>>
    {
        [IntentManaged(Mode.Merge)]
        public PersonUpdatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            DomainEventNotification<PersonUpdatedEvent> notification,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}