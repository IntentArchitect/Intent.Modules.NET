using AutoMapper;
using CqrsSpike.Subscriber.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipments
{
    public class GetShipmentsQueryHandler : IQueryHandler<GetShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async ValueTask<List<ShipmentDto>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = await _shipmentRepository.FindAllAsync(cancellationToken);
            return shipments.MapToShipmentDtoList(_mapper);
        }
    }
}