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
    public static class AA4_Composite_ManyDTOMappingExtensions
    {
        public static AA4_Composite_ManyDTO MapToAA4_Composite_ManyDTO(this IAA4_Composite_Many projectFrom, IMapper mapper)
        {
            return mapper.Map<AA4_Composite_ManyDTO>(projectFrom);
        }

        public static List<AA4_Composite_ManyDTO> MapToAA4_Composite_ManyDTOList(this IEnumerable<IAA4_Composite_Many> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAA4_Composite_ManyDTO(mapper)).ToList();
        }
    }
}