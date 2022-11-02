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
    public static class AA3_Aggregation_SingleDTOMappingExtensions
    {
        public static AA3_Aggregation_SingleDTO MapToAA3_Aggregation_SingleDTO(this IAA3_Aggregation_Single projectFrom, IMapper mapper)
        {
            return mapper.Map<AA3_Aggregation_SingleDTO>(projectFrom);
        }

        public static List<AA3_Aggregation_SingleDTO> MapToAA3_Aggregation_SingleDTOList(this IEnumerable<IAA3_Aggregation_Single> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAA3_Aggregation_SingleDTO(mapper)).ToList();
        }
    }
}