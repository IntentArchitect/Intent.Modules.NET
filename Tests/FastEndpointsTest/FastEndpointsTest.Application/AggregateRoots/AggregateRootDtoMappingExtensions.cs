using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public static class AggregateRootDtoMappingExtensions
    {
        public static AggregateRootDto MapToAggregateRootDto(this AggregateRoot projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootDto>(projectFrom);

        public static List<AggregateRootDto> MapToAggregateRootDtoList(this IEnumerable<AggregateRoot> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootDto(mapper)).ToList();
    }
}