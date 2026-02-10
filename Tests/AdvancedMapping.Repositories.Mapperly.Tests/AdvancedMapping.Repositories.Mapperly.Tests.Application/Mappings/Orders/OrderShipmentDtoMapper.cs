using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderShipmentDtoMapper
    {
        [MapperIgnoreSource(nameof(Shipment.OrderId))]
        public partial OrderShipmentDto ShipmentToOrderShipmentDto(Shipment shipment);

        public partial List<OrderShipmentDto> ShipmentToOrderShipmentDtoList(List<Shipment> shipments);
    }
}