using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Application.Shipments.CreateShipment;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RequestShipmentCommandHandler : IIntegrationEventHandler<RequestShipmentCommand>
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public RequestShipmentCommandHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(RequestShipmentCommand message, CancellationToken cancellationToken = default)
        {
            var command = new CreateShipmentCommand(
                orderId: message.OrderId);
            var result = await _mediator.Send(command, cancellationToken);
        }
    }
}