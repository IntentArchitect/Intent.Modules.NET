using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder;
using Mediator;

namespace CqrsSpike.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ISender _mediator;

        public OrderCreatedEventHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new RecordOrderCommand(
                orderId: message.OrderId,
                customerName: message.CustomerName,
                total: message.Total);
            await _mediator.Send(command, cancellationToken);
        }
    }
}