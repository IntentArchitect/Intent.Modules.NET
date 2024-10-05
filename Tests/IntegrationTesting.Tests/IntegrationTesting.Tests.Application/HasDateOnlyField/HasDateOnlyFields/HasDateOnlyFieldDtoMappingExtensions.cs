using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields
{
    public static class HasDateOnlyFieldDtoMappingExtensions
    {
        public static HasDateOnlyFieldDto MapToHasDateOnlyFieldDto(this Domain.Entities.HasDateOnlyField projectFrom, IMapper mapper)
            => mapper.Map<HasDateOnlyFieldDto>(projectFrom);

        public static List<HasDateOnlyFieldDto> MapToHasDateOnlyFieldDtoList(this IEnumerable<Domain.Entities.HasDateOnlyField> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToHasDateOnlyFieldDto(mapper)).ToList();
    }
}