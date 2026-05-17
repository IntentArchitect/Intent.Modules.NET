using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.GetShipmentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, ShipmentDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShipmentDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetShipmentByIdQueryHandler(IOrderRepository orderRepository, ShipmentDtoMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ShipmentDto> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.OrderId}'");
            }

            var shipment = order.Shipments.FirstOrDefault(x => x.Id == request.Id);
            if (shipment is null)
            {
                throw new NotFoundException($"Could not find Shipment '{request.Id}'");
            }
            return _mapper.ShipmentToShipmentDto(shipment);
        }
    }
}