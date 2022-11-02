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
    public static class A_Composite_SingleDTOMappingExtensions
    {
        public static A_Composite_SingleDTO MapToA_Composite_SingleDTO(this IA_Composite_Single projectFrom, IMapper mapper)
        {
            return mapper.Map<A_Composite_SingleDTO>(projectFrom);
        }

        public static List<A_Composite_SingleDTO> MapToA_Composite_SingleDTOList(this IEnumerable<IA_Composite_Single> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToA_Composite_SingleDTO(mapper)).ToList();
        }
    }
}