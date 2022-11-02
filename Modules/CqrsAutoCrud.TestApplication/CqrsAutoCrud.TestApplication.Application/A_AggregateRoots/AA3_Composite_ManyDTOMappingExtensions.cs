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
    public static class AA3_Composite_ManyDTOMappingExtensions
    {
        public static AA3_Composite_ManyDTO MapToAA3_Composite_ManyDTO(this IAA3_Composite_Many projectFrom, IMapper mapper)
        {
            return mapper.Map<AA3_Composite_ManyDTO>(projectFrom);
        }

        public static List<AA3_Composite_ManyDTO> MapToAA3_Composite_ManyDTOList(this IEnumerable<IAA3_Composite_Many> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAA3_Composite_ManyDTO(mapper)).ToList();
        }
    }
}