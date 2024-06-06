using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
{
    public static class AggregateRootLongDtoMappingExtensions
    {
        public static AggregateRootLongDto MapToAggregateRootLongDto(this AggregateRootLong projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootLongDto>(projectFrom);

        public static List<AggregateRootLongDto> MapToAggregateRootLongDtoList(this IEnumerable<AggregateRootLong> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootLongDto(mapper)).ToList();
    }
}