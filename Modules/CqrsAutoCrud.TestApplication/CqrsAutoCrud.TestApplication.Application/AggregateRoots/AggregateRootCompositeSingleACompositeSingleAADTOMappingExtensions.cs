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
    public static class AggregateRootCompositeSingleACompositeSingleAADTOMappingExtensions
    {
        public static AggregateRootCompositeSingleACompositeSingleAADTO MapToAggregateRootCompositeSingleACompositeSingleAADTO(this CompositeSingleAA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeSingleACompositeSingleAADTO>(projectFrom);
        }

        public static List<AggregateRootCompositeSingleACompositeSingleAADTO> MapToAggregateRootCompositeSingleACompositeSingleAADTOList(this IEnumerable<CompositeSingleAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeSingleACompositeSingleAADTO(mapper)).ToList();
        }
    }
}