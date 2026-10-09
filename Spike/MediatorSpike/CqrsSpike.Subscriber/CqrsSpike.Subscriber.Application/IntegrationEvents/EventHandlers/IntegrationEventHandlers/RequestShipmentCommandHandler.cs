using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Application.Shipments.CreateShipment;
using CqrsSpike.Subscriber.Eventing.Messages;
using Mediator;

namespace CqrsSpike.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    public class RequestShipmentCommandHandler : IIntegrationEventHandler<RequestShipmentCommand>
    {
        private readonly ISender _mediator;

        public RequestShipmentCommandHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task HandleAsync(RequestShipmentCommand message, CancellationToken cancellationToken = default)
        {
            var command = new CreateShipmentCommand(
                orderId: message.OrderId);
            var result = await _mediator.Send(command, cancellationToken);
        }
    }
}