using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals
{
    public static class UniqueConValDtoMappingExtensions
    {
        public static UniqueConValDto MapToUniqueConValDto(this UniqueConVal projectFrom, IMapper mapper)
            => mapper.Map<UniqueConValDto>(projectFrom);

        public static List<UniqueConValDto> MapToUniqueConValDtoList(this IEnumerable<UniqueConVal> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUniqueConValDto(mapper)).ToList();
    }
}