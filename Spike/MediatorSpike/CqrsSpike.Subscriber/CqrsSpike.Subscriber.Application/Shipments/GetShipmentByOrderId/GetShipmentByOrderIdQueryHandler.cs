using AutoMapper;
using CqrsSpike.Subscriber.Domain.Common.Exceptions;
using CqrsSpike.Subscriber.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments.GetShipmentByOrderId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetShipmentByOrderIdQueryHandler : IRequestHandler<GetShipmentByOrderIdQuery, ShipmentDto>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetShipmentByOrderIdQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ShipmentDto> Handle(GetShipmentByOrderIdQuery request, CancellationToken cancellationToken)
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