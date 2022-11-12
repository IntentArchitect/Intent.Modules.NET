using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{
    public static class UpdateCompositeSingleADTOMappingExtensions
    {
        public static UpdateCompositeSingleADTO MapToUpdateCompositeSingleADTO(this CompositeSingleA projectFrom, IMapper mapper)
        {
            return mapper.Map<UpdateCompositeSingleADTO>(projectFrom);
        }

        public static List<UpdateCompositeSingleADTO> MapToUpdateCompositeSingleADTOList(this IEnumerable<CompositeSingleA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToUpdateCompositeSingleADTO(mapper)).ToList();
        }
    }
}