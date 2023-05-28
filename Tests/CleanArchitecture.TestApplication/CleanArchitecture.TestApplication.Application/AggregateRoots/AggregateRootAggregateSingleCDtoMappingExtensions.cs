using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{
    public static class AggregateRootAggregateSingleCDtoMappingExtensions
    {
        public static AggregateRootAggregateSingleCDto MapToAggregateRootAggregateSingleCDto(this AggregateSingleC projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootAggregateSingleCDto>(projectFrom);

        public static List<AggregateRootAggregateSingleCDto> MapToAggregateRootAggregateSingleCDtoList(this IEnumerable<AggregateSingleC> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootAggregateSingleCDto(mapper)).ToList();
    }
}