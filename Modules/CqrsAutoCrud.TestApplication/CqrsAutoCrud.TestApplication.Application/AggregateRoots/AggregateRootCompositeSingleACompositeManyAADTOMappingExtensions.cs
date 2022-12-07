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
    public static class AggregateRootCompositeSingleACompositeManyAADTOMappingExtensions
    {
        public static AggregateRootCompositeSingleACompositeManyAADTO MapToAggregateRootCompositeSingleACompositeManyAADTO(this CompositeManyAA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeSingleACompositeManyAADTO>(projectFrom);
        }

        public static List<AggregateRootCompositeSingleACompositeManyAADTO> MapToAggregateRootCompositeSingleACompositeManyAADTOList(this IEnumerable<CompositeManyAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeSingleACompositeManyAADTO(mapper)).ToList();
        }
    }
}