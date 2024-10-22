using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Models;
using SharedKernel.Consumer.Tests.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.EventHandlers.Orders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderCreatedHandler : INotificationHandler<DomainEventNotification<OrderCreated>>
    {
        [IntentManaged(Mode.Merge)]
        public OrderCreatedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DomainEventNotification<OrderCreated> notification, CancellationToken cancellationToken)
        {
        }
    }
}