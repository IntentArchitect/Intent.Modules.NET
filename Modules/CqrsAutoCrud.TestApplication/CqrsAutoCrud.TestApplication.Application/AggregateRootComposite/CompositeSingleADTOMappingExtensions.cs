using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootComposite
{
    public static class CompositeSingleADTOMappingExtensions
    {
        public static CompositeSingleADTO MapToCompositeSingleADTO(this CompositeSingleA projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeSingleADTO>(projectFrom);
        }

        public static List<CompositeSingleADTO> MapToCompositeSingleADTOList(this IEnumerable<CompositeSingleA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeSingleADTO(mapper)).ToList();
        }
    }
}