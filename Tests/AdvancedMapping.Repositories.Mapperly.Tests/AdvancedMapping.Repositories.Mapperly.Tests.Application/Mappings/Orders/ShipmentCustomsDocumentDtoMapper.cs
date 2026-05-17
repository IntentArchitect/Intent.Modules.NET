using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class ShipmentCustomsDocumentDtoMapper
    {
        [MapperIgnoreSource(nameof(CustomsDocument.CustomsId))]
        public partial ShipmentCustomsDocumentDto CustomsDocumentToShipmentCustomsDocumentDto(CustomsDocument customsDocument);

        public partial List<ShipmentCustomsDocumentDto> CustomsDocumentToShipmentCustomsDocumentDtoList(IEnumerable<CustomsDocument> customsDocuments);
    }
}