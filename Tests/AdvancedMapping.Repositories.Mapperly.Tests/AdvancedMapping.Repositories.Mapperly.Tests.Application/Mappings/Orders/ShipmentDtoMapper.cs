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
        private readonly ShipmentCustomsDocumentDtoMapper _shipmentCustomsDocumentDtoMapper;

        public ShipmentDtoMapper(ShipmentCustomsDocumentDtoMapper shipmentCustomsDocumentDtoMapper)
        {
            _shipmentCustomsDocumentDtoMapper = shipmentCustomsDocumentDtoMapper;
        }
        [MapperIgnoreSource(nameof(Shipment.CustomsId))]
        [MapProperty(nameof(@Shipment.Dispatch.Document.DocumentNumber), nameof(ShipmentDto.DispatchDocumentNumber))]
        [MapProperty(nameof(@Shipment.Manifest.Document.DocumentNumber), nameof(ShipmentDto.ManifestDocumentNumber))]
        [MapPropertyFromSource(nameof(ShipmentDto.CustomsDocuments), Use = nameof(MapCustomsDocuments))]
        public partial ShipmentDto ShipmentToShipmentDto(Shipment shipment);

        public partial List<ShipmentDto> ShipmentToShipmentDtoList(IEnumerable<Shipment> shipments);

        private IEnumerable<ShipmentCustomsDocumentDto> MapCustomsDocuments(Shipment source) => source.Customs!.CustomsDocuments.Select(_shipmentCustomsDocumentDtoMapper.CustomsDocumentToShipmentCustomsDocumentDto);
    }
}