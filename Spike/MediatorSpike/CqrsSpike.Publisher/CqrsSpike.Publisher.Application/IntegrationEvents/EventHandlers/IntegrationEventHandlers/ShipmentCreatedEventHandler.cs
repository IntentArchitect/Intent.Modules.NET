using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Application.Orders.MarkOrderShipped;
using CqrsSpike.Subscriber.Eventing.Messages;
using Mediator;

namespace CqrsSpike.Publisher.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    public class ShipmentCreatedEventHandler : IIntegrationEventHandler<ShipmentCreatedEvent>
    {
        private readonly ISender _mediator;

        public ShipmentCreatedEventHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task HandleAsync(ShipmentCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new MarkOrderShippedCommand(
                orderId: message.OrderId,
                shipmentId: message.ShipmentId);
            await _mediator.Send(command, cancellationToken);
        }
    }
}