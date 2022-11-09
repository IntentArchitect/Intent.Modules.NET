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
    public static class CompositeManyBDTOMappingExtensions
    {
        public static CompositeManyBDTO MapToCompositeManyBDTO(this CompositeManyB projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeManyBDTO>(projectFrom);
        }

        public static List<CompositeManyBDTO> MapToCompositeManyBDTOList(this IEnumerable<CompositeManyB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeManyBDTO(mapper)).ToList();
        }
    }
}