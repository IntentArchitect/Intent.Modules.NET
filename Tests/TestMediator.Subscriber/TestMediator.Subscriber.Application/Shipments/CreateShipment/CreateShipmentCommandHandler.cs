using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Subscriber.Application.Common.Eventing;
using TestMediator.Subscriber.Domain;
using TestMediator.Subscriber.Domain.Repositories;
using TestMediator.Subscriber.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Subscriber.Application.Shipments.CreateShipment
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = new Shipment { Id = Guid.NewGuid(), OrderId = request.OrderId };
            _shipmentRepository.Add(shipment);
            _messageBus.Publish(new ShipmentCreatedEvent { ShipmentId = shipment.Id, OrderId = request.OrderId });
            await Task.CompletedTask;
            return shipment.Id;
        }
    }
}
