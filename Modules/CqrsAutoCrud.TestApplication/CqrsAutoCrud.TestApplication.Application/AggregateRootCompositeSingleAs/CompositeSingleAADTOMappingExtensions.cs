using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs
{
    public static class CompositeSingleAADTOMappingExtensions
    {
        public static CompositeSingleAADTO MapToCompositeSingleAADTO(this CompositeSingleAA projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeSingleAADTO>(projectFrom);
        }

        public static List<CompositeSingleAADTO> MapToCompositeSingleAADTOList(this IEnumerable<CompositeSingleAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeSingleAADTO(mapper)).ToList();
        }
    }
}