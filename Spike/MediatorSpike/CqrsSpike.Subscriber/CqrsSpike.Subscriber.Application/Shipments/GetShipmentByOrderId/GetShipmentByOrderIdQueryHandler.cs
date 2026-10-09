using AutoMapper;
using CqrsSpike.Subscriber.Domain.Common.Exceptions;
using CqrsSpike.Subscriber.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    public class GetShipmentByOrderIdQueryHandler : IQueryHandler<GetShipmentByOrderIdQuery, ShipmentDto>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public GetShipmentByOrderIdQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async ValueTask<ShipmentDto> Handle(GetShipmentByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.FindAsync(x => x.OrderId == request.OrderId, cancellationToken);
            if (shipment is null)
            {
                throw new NotFoundException($"Could not find Shipment '{request.OrderId}'");
            }
            return shipment.MapToShipmentDto(_mapper);
        }
    }
}