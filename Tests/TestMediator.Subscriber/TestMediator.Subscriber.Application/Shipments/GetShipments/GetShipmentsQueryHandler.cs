using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Subscriber.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace TestMediator.Subscriber.Application.Shipments.GetShipments
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetShipmentsQueryHandler : IQueryHandler<GetShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IShipmentRepository _shipmentRepository;

        [IntentManaged(Mode.Merge)]
        public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<List<ShipmentDto>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = await _shipmentRepository.FindAllAsync(cancellationToken);
            return shipments.Select(s => new ShipmentDto { Id = s.Id, OrderId = s.OrderId }).ToList();
        }
    }
}
