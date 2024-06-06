using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public static class AggregateRootCompositeSingleADtoMappingExtensions
    {
        public static AggregateRootCompositeSingleADto MapToAggregateRootCompositeSingleADto(this CompositeSingleA projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeSingleADto>(projectFrom);

        public static List<AggregateRootCompositeSingleADto> MapToAggregateRootCompositeSingleADtoList(this IEnumerable<CompositeSingleA> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeSingleADto(mapper)).ToList();
    }
}