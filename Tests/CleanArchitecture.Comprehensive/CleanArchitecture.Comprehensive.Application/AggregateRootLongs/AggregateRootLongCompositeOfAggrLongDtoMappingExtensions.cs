using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
{
    public static class AggregateRootLongCompositeOfAggrLongDtoMappingExtensions
    {
        public static AggregateRootLongCompositeOfAggrLongDto MapToAggregateRootLongCompositeOfAggrLongDto(this CompositeOfAggrLong projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootLongCompositeOfAggrLongDto>(projectFrom);

        public static List<AggregateRootLongCompositeOfAggrLongDto> MapToAggregateRootLongCompositeOfAggrLongDtoList(this IEnumerable<CompositeOfAggrLong> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootLongCompositeOfAggrLongDto(mapper)).ToList();
    }
}