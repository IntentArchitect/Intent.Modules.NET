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
        [UseMapper]
        private readonly ShipmentVesselDtoMapper _shipmentVesselDtoMapper;

        public ShipmentDtoMapper(ShipmentVesselDtoMapper shipmentVesselDtoMapper)
        {
            _shipmentVesselDtoMapper = shipmentVesselDtoMapper;
        }
        [MapperIgnoreSource(nameof(Shipment.ContainerId))]
        [MapProperty(nameof(@Shipment.Dispatch.Document.DocumentNumber), nameof(ShipmentDto.DispatchDocumentNumber))]
        [MapProperty(nameof(@Shipment.Manifest.Document.DocumentNumber), nameof(ShipmentDto.ManifestDocumentNumber))]
        [MapPropertyFromSource(nameof(ShipmentDto.Vessels), Use = nameof(MapVessels))]
        public partial ShipmentDto ShipmentToShipmentDto(Shipment shipment);

        public partial List<ShipmentDto> ShipmentToShipmentDtoList(IEnumerable<Shipment> shipments);

        private IEnumerable<ShipmentVesselDto> MapVessels(Shipment source) => source.Container!.Vessels.Select(_shipmentVesselDtoMapper.VesselToShipmentVesselDto);
    }
}