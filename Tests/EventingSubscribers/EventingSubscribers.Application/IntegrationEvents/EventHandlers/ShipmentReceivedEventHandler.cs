using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain;
using EventingSubscribers.Domain.Common.Exceptions;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ShipmentReceivedEventHandler : IIntegrationEventHandler<ShipmentReceivedEvent>
    {
        private readonly IShipmentRepository _shipmentRepository;

        [IntentManaged(Mode.Merge)]
        public ShipmentReceivedEventHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ShipmentReceivedEvent message, CancellationToken cancellationToken = default)
        {
            var updateShipment = await _shipmentRepository.FindByIdAsync(message.ShipmentId, cancellationToken);
            if (updateShipment is null)
            {
                throw new NotFoundException($"Could not find Shipment '{message.ShipmentId}'");
            }

            updateShipment.Title = message.Title;
            updateShipment.DestinationAddress = new Address(
                street: message.DestinationStreet,
                city: message.DestinationCity);
        }
    }
}