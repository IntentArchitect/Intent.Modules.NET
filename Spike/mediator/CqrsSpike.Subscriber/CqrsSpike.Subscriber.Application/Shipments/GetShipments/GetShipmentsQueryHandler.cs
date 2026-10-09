using AutoMapper;
using CqrsSpike.Subscriber.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetShipmentsQueryHandler : IQueryHandler<GetShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async ValueTask<List<ShipmentDto>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = await _shipmentRepository.FindAllAsync(cancellationToken);
            return shipments.MapToShipmentDtoList(_mapper);
        }
    }
}