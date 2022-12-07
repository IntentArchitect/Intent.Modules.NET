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
    public static class AggregateRootCompositeSingleADTOMappingExtensions
    {
        public static AggregateRootCompositeSingleADTO MapToAggregateRootCompositeSingleADTO(this CompositeSingleA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeSingleADTO>(projectFrom);
        }

        public static List<AggregateRootCompositeSingleADTO> MapToAggregateRootCompositeSingleADTOList(this IEnumerable<CompositeSingleA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeSingleADTO(mapper)).ToList();
        }
    }
}