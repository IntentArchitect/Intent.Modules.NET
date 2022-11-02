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
    public static class AA2_Composite_SingleDTOMappingExtensions
    {
        public static AA2_Composite_SingleDTO MapToAA2_Composite_SingleDTO(this IAA2_Composite_Single projectFrom, IMapper mapper)
        {
            return mapper.Map<AA2_Composite_SingleDTO>(projectFrom);
        }

        public static List<AA2_Composite_SingleDTO> MapToAA2_Composite_SingleDTOList(this IEnumerable<IAA2_Composite_Single> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAA2_Composite_SingleDTO(mapper)).ToList();
        }
    }
}