using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{
    public static class AggregateRootLongDTOMappingExtensions
    {
        public static AggregateRootLongDTO MapToAggregateRootLongDTO(this AggregateRootLong projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootLongDTO>(projectFrom);
        }

        public static List<AggregateRootLongDTO> MapToAggregateRootLongDTOList(this IEnumerable<AggregateRootLong> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootLongDTO(mapper)).ToList();
        }
    }
}