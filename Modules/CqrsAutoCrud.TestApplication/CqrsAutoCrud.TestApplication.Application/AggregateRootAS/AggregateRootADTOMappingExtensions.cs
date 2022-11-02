using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS
{
    public static class AggregateRootADTOMappingExtensions
    {
        public static AggregateRootADTO MapToAggregateRootADTO(this IAggregateRootA projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootADTO>(projectFrom);
        }

        public static List<AggregateRootADTO> MapToAggregateRootADTOList(this IEnumerable<IAggregateRootA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootADTO(mapper)).ToList();
        }
    }
}