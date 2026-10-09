using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.CreateShipment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
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
        public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
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