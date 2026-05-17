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
        [MapProperty(nameof(@Shipment.Dispatch.Document.DocumentNumber), nameof(OrderShipmentDto.DispatchDocumentNumber))]
        [MapProperty(nameof(@Shipment.Manifest.Document.DocumentNumber), nameof(OrderShipmentDto.ManifestDocumentNumber))]
        public partial OrderShipmentDto ShipmentToOrderShipmentDto(Shipment shipment);

        public partial List<OrderShipmentDto> ShipmentToOrderShipmentDtoList(IEnumerable<Shipment> shipments);
    }
}