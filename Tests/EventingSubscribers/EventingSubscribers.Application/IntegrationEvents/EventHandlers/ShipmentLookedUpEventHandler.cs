using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Common.Exceptions;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ShipmentLookedUpEventHandler : IIntegrationEventHandler<ShipmentLookedUpEvent>
    {
        private readonly IShipmentRepository _shipmentRepository;

        [IntentManaged(Mode.Merge)]
        public ShipmentLookedUpEventHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ShipmentLookedUpEvent message, CancellationToken cancellationToken = default)
        {
            var queryShipment = await _shipmentRepository.FindByIdAsync(message.ShipmentId, cancellationToken);
            if (queryShipment is null)
            {
                throw new NotFoundException($"Could not find Shipment '{message.ShipmentId}'");
            }
        }
    }
}