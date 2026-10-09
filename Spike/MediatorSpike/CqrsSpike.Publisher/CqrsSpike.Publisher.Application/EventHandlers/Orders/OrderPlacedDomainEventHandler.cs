using CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.EventHandlers.Orders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderPlacedDomainEventHandler : INotificationHandler<DomainEventNotification<OrderPlacedDomainEvent>>
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public OrderPlacedDomainEventHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(
            DomainEventNotification<OrderPlacedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            await _mediator.Send(new CreateAuditEntryCommand(
                description: $"Order placed for {domainEvent.Order.CustomerName}"), cancellationToken);
        }
    }
}