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
    public static class CreateAggregateSingleCDTOMappingExtensions
    {
        public static CreateAggregateSingleCDTO MapToCreateAggregateSingleCDTO(this IAggregateSingleC projectFrom, IMapper mapper)
        {
            return mapper.Map<CreateAggregateSingleCDTO>(projectFrom);
        }

        public static List<CreateAggregateSingleCDTO> MapToCreateAggregateSingleCDTOList(this IEnumerable<IAggregateSingleC> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCreateAggregateSingleCDTO(mapper)).ToList();
        }
    }
}