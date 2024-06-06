using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public static class AggregateRootCompositeSingleACompositeSingleAADtoMappingExtensions
    {
        public static AggregateRootCompositeSingleACompositeSingleAADto MapToAggregateRootCompositeSingleACompositeSingleAADto(this CompositeSingleAA projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeSingleACompositeSingleAADto>(projectFrom);

        public static List<AggregateRootCompositeSingleACompositeSingleAADto> MapToAggregateRootCompositeSingleACompositeSingleAADtoList(this IEnumerable<CompositeSingleAA> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeSingleACompositeSingleAADto(mapper)).ToList();
    }
}