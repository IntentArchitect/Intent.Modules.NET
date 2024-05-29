using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public static class AggregateRootCompositeSingleACompositeManyAADtoMappingExtensions
    {
        public static AggregateRootCompositeSingleACompositeManyAADto MapToAggregateRootCompositeSingleACompositeManyAADto(this CompositeManyAA projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeSingleACompositeManyAADto>(projectFrom);

        public static List<AggregateRootCompositeSingleACompositeManyAADto> MapToAggregateRootCompositeSingleACompositeManyAADtoList(this IEnumerable<CompositeManyAA> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeSingleACompositeManyAADto(mapper)).ToList();
    }
}