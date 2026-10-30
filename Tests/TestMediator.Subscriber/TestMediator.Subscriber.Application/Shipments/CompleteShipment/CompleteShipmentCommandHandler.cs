using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Subscriber.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Subscriber.Application.Shipments.CompleteShipment
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CompleteShipmentCommandHandler : ICommandHandler<CompleteShipmentCommand>
    {
        private readonly IShipmentRepository _shipmentRepository;

        [IntentManaged(Mode.Merge)]
        public CompleteShipmentCommandHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(CompleteShipmentCommand request, CancellationToken cancellationToken)
        {
            // The shipment for the order, if one was recorded, is simply left in place: completion has no state change to model.
            _ = await _shipmentRepository.FindAsync(s => s.OrderId == request.OrderId, cancellationToken);
            return Unit.Value;
        }
    }
}
