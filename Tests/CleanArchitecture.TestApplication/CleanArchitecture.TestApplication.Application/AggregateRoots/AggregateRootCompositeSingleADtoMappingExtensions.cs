using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{
    public static class AggregateRootCompositeSingleADtoMappingExtensions
    {
        public static AggregateRootCompositeSingleADto MapToAggregateRootCompositeSingleADto(this CompositeSingleA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeSingleADto>(projectFrom);
        }

        public static List<AggregateRootCompositeSingleADto> MapToAggregateRootCompositeSingleADtoList(this IEnumerable<CompositeSingleA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeSingleADto(mapper)).ToList();
        }
    }
}