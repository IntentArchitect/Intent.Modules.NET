using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Application.Orders.MarkOrderShipped;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ShipmentCreatedEventHandler : IIntegrationEventHandler<ShipmentCreatedEvent>
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public ShipmentCreatedEventHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ShipmentCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new MarkOrderShippedCommand(
                orderId: message.OrderId,
                shipmentId: message.ShipmentId);
            await _mediator.Send(command, cancellationToken);
        }
    }
}