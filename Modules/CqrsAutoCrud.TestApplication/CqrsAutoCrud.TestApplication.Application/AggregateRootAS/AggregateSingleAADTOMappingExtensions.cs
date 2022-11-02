using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS
{
    public static class AggregateSingleAADTOMappingExtensions
    {
        public static AggregateSingleAADTO MapToAggregateSingleAADTO(this IAggregateSingleAA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateSingleAADTO>(projectFrom);
        }

        public static List<AggregateSingleAADTO> MapToAggregateSingleAADTOList(this IEnumerable<IAggregateSingleAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateSingleAADTO(mapper)).ToList();
        }
    }
}