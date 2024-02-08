using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds
{
    public static class PartialCrudDtoMappingExtensions
    {
        public static PartialCrudDto MapToPartialCrudDto(this PartialCrud projectFrom, IMapper mapper)
            => mapper.Map<PartialCrudDto>(projectFrom);

        public static List<PartialCrudDto> MapToPartialCrudDtoList(this IEnumerable<PartialCrud> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPartialCrudDto(mapper)).ToList();
    }
}