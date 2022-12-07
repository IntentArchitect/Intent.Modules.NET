using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{
    public static class AggregateRootAggregateSingleCDTOMappingExtensions
    {
        public static AggregateRootAggregateSingleCDTO MapToAggregateRootAggregateSingleCDTO(this AggregateSingleC projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootAggregateSingleCDTO>(projectFrom);
        }

        public static List<AggregateRootAggregateSingleCDTO> MapToAggregateRootAggregateSingleCDTOList(this IEnumerable<AggregateSingleC> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootAggregateSingleCDTO(mapper)).ToList();
        }
    }
}