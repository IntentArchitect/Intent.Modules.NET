using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderCustomerSegmentsDtoMapper
    {
        public partial OrderCustomerSegmentsDto CustomerSegmentsToOrderCustomerSegmentsDto(Domain.Entities.CustomerSegments customerSegments);

        public partial List<OrderCustomerSegmentsDto> CustomerSegmentsToOrderCustomerSegmentsDtoList(IEnumerable<Domain.Entities.CustomerSegments> customerSegments);
    }
}