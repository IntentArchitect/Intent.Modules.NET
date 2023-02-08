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
    public static class AggregateRootCompositeSingleACompositeSingleAADtoMappingExtensions
    {
        public static AggregateRootCompositeSingleACompositeSingleAADto MapToAggregateRootCompositeSingleACompositeSingleAADto(this CompositeSingleAA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeSingleACompositeSingleAADto>(projectFrom);
        }

        public static List<AggregateRootCompositeSingleACompositeSingleAADto> MapToAggregateRootCompositeSingleACompositeSingleAADtoList(this IEnumerable<CompositeSingleAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeSingleACompositeSingleAADto(mapper)).ToList();
        }
    }
}