using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public static class AggregateRootCompositeManyBCompositeSingleBBDtoMappingExtensions
    {
        public static AggregateRootCompositeManyBCompositeSingleBBDto MapToAggregateRootCompositeManyBCompositeSingleBBDto(this CompositeSingleBB projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeManyBCompositeSingleBBDto>(projectFrom);

        public static List<AggregateRootCompositeManyBCompositeSingleBBDto> MapToAggregateRootCompositeManyBCompositeSingleBBDtoList(this IEnumerable<CompositeSingleBB> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeManyBCompositeSingleBBDto(mapper)).ToList();
    }
}