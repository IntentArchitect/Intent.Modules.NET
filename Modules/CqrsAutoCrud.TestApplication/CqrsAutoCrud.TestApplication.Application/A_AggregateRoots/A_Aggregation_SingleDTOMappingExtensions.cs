using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots
{
    public static class A_Aggregation_SingleDTOMappingExtensions
    {
        public static A_Aggregation_SingleDTO MapToA_Aggregation_SingleDTO(this IA_Aggregation_Single projectFrom, IMapper mapper)
        {
            return mapper.Map<A_Aggregation_SingleDTO>(projectFrom);
        }

        public static List<A_Aggregation_SingleDTO> MapToA_Aggregation_SingleDTOList(this IEnumerable<IA_Aggregation_Single> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToA_Aggregation_SingleDTO(mapper)).ToList();
        }
    }
}