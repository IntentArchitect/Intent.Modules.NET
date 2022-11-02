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
    public static class A_Composite_ManyDTOMappingExtensions
    {
        public static A_Composite_ManyDTO MapToA_Composite_ManyDTO(this IA_Composite_Many projectFrom, IMapper mapper)
        {
            return mapper.Map<A_Composite_ManyDTO>(projectFrom);
        }

        public static List<A_Composite_ManyDTO> MapToA_Composite_ManyDTOList(this IEnumerable<IA_Composite_Many> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToA_Composite_ManyDTO(mapper)).ToList();
        }
    }
}