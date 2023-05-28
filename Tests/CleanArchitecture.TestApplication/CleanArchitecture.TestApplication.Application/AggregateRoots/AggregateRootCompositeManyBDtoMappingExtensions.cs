using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{
    public static class AggregateRootCompositeManyBDtoMappingExtensions
    {
        public static AggregateRootCompositeManyBDto MapToAggregateRootCompositeManyBDto(this CompositeManyB projectFrom, IMapper mapper)
            => mapper.Map<AggregateRootCompositeManyBDto>(projectFrom);

        public static List<AggregateRootCompositeManyBDto> MapToAggregateRootCompositeManyBDtoList(this IEnumerable<CompositeManyB> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateRootCompositeManyBDto(mapper)).ToList();
    }
}