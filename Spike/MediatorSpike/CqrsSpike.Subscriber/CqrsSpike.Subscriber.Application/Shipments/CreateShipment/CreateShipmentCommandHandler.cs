using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using CqrsSpike.Subscriber.Eventing.Messages;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    public class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Guid>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMessageBus _messageBus;

        public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IMessageBus messageBus)
        {
            _shipmentRepository = shipmentRepository;
            _messageBus = messageBus;
        }

        public async ValueTask<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = new Shipment
            {
                OrderId = request.OrderId,
                Status = "Created",
                CreatedAt = DateTimeOffset.UtcNow
            };

            _shipmentRepository.Add(shipment);
            await _shipmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _messageBus.Publish(new ShipmentCreatedEvent
            {
                ShipmentId = shipment.Id,
                OrderId = request.OrderId
            });
            return shipment.Id;
        }
    }
}