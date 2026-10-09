using CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Domain.Events;
using Mediator;

namespace CqrsSpike.Publisher.Application.EventHandlers.Orders
{
    public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        private readonly ISender _mediator;

        public OrderPlacedDomainEventHandler(ISender mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask Handle(
            OrderPlacedDomainEventNotification notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            await _mediator.Send(new CreateAuditEntryCommand(
                description: $"Order placed for {domainEvent.Order.CustomerName}"), cancellationToken);
        }
    }
}