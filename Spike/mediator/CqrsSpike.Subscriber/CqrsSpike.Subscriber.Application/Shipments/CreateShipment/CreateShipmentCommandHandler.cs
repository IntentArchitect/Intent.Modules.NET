using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Guid>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, IMessageBus messageBus)
        {
            _shipmentRepository = shipmentRepository;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
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