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
    public static class CompositeManyBBDTOMappingExtensions
    {
        public static CompositeManyBBDTO MapToCompositeManyBBDTO(this CompositeManyBB projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeManyBBDTO>(projectFrom);
        }

        public static List<CompositeManyBBDTO> MapToCompositeManyBBDTOList(this IEnumerable<CompositeManyBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeManyBBDTO(mapper)).ToList();
        }
    }
}