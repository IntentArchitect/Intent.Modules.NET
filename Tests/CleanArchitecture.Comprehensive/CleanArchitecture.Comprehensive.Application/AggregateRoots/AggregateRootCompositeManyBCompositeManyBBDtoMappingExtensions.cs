using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public static class AggregateRootCompositeManyBCompositeManyBBDtoMappingExtensions
    {
        public static AggregateRootCompositeManyBCompositeManyBBDto MapToAggregateRootCompositeManyBCompositeManyBBDto(this CompositeManyBB projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeManyBCompositeManyBBDto>(projectFrom);

        public static List<AggregateRootCompositeManyBCompositeManyBBDto> MapToAggregateRootCompositeManyBCompositeManyBBDtoList(this IEnumerable<CompositeManyBB> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeManyBCompositeManyBBDto(mapper)).ToList();
    }
}