using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Models;
using CleanArchitecture.TestApplication.Domain.Events;
using CleanArchitecture.TestApplication.Domain.Events.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EventHandlers.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DataContractDomainEventHandler : INotificationHandler<DomainEventNotification<DataContractDomainEvent>>
    {
        public DataContractDomainEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DomainEventNotification<DataContractDomainEvent> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}