using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns
{
    public static class AggregateTestNoIdReturnDtoMappingExtensions
    {
        public static AggregateTestNoIdReturnDto MapToAggregateTestNoIdReturnDto(this AggregateTestNoIdReturn projectFrom, IMapper mapper)
            => mapper.Map<AggregateTestNoIdReturnDto>(projectFrom);

        public static List<AggregateTestNoIdReturnDto> MapToAggregateTestNoIdReturnDtoList(this IEnumerable<AggregateTestNoIdReturn> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAggregateTestNoIdReturnDto(mapper)).ToList();
    }
}