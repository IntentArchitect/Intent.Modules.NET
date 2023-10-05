using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.OnlyModeledDomainEvents.Application.Common.Models;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerCreatedHandler : INotificationHandler<DomainEventNotification<CustomerCreated>>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerCreatedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            DomainEventNotification<CustomerCreated> notification,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}