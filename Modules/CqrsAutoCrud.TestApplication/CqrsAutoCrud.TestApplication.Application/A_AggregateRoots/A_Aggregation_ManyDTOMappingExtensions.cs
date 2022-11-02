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
    public static class A_Aggregation_ManyDTOMappingExtensions
    {
        public static A_Aggregation_ManyDTO MapToA_Aggregation_ManyDTO(this IA_Aggregation_Many projectFrom, IMapper mapper)
        {
            return mapper.Map<A_Aggregation_ManyDTO>(projectFrom);
        }

        public static List<A_Aggregation_ManyDTO> MapToA_Aggregation_ManyDTOList(this IEnumerable<IA_Aggregation_Many> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToA_Aggregation_ManyDTO(mapper)).ToList();
        }
    }
}