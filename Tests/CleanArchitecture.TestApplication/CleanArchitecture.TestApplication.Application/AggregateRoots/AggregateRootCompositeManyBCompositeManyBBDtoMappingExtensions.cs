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
    public static class AggregateRootCompositeManyBCompositeManyBBDtoMappingExtensions
    {
        public static AggregateRootCompositeManyBCompositeManyBBDto MapToAggregateRootCompositeManyBCompositeManyBBDto(this CompositeManyBB projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeManyBCompositeManyBBDto>(projectFrom);
        }

        public static List<AggregateRootCompositeManyBCompositeManyBBDto> MapToAggregateRootCompositeManyBCompositeManyBBDtoList(this IEnumerable<CompositeManyBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeManyBCompositeManyBBDto(mapper)).ToList();
        }
    }
}