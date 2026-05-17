using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class ShipmentDtoMapper
    {
        [MapProperty(nameof(@Shipment.Dispatch.Document.DocumentNumber), nameof(ShipmentDto.DispatchDocumentNumber))]
        [MapProperty(nameof(@Shipment.Manifest.Document.DocumentNumber), nameof(ShipmentDto.ManifestDocumentNumber))]
        public partial ShipmentDto ShipmentToShipmentDto(Shipment shipment);

        public partial List<ShipmentDto> ShipmentToShipmentDtoList(IEnumerable<Shipment> shipments);
    }
}