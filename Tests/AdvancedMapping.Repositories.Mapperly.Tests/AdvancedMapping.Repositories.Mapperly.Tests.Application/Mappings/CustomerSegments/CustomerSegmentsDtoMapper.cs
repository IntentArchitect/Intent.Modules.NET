using AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.CustomerSegments
{
    [Mapper]
    public partial class CustomerSegmentsDtoMapper
    {
        public partial CustomerSegmentsDto CustomerSegmentsToCustomerSegmentsDto(Domain.Entities.CustomerSegments customerSegments);

        public partial List<CustomerSegmentsDto> CustomerSegmentsToCustomerSegmentsDtoList(IEnumerable<Domain.Entities.CustomerSegments> customerSegments);
    }
}