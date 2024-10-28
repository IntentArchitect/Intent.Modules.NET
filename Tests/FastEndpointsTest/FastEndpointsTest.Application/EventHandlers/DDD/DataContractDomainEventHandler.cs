using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Common.Models;
using FastEndpointsTest.Domain.Events.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.EventHandlers.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DataContractDomainEventHandler : INotificationHandler<DomainEventNotification<DataContractDomainEvent>>
    {
        [IntentManaged(Mode.Merge)]
        public DataContractDomainEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            DomainEventNotification<DataContractDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle DataContractDomainEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}