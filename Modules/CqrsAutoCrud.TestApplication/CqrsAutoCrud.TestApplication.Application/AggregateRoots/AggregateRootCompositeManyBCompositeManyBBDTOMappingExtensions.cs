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
    public static class AggregateRootCompositeManyBCompositeManyBBDTOMappingExtensions
    {
        public static AggregateRootCompositeManyBCompositeManyBBDTO MapToAggregateRootCompositeManyBCompositeManyBBDTO(this CompositeManyBB projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeManyBCompositeManyBBDTO>(projectFrom);
        }

        public static List<AggregateRootCompositeManyBCompositeManyBBDTO> MapToAggregateRootCompositeManyBCompositeManyBBDTOList(this IEnumerable<CompositeManyBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeManyBCompositeManyBBDTO(mapper)).ToList();
        }
    }
}