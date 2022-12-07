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
    public static class AggregateRootCompositeManyBCompositeSingleBBDTOMappingExtensions
    {
        public static AggregateRootCompositeManyBCompositeSingleBBDTO MapToAggregateRootCompositeManyBCompositeSingleBBDTO(this CompositeSingleBB projectFrom, IMapper mapper)
        {
            return mapper.Map<AggregateRootCompositeManyBCompositeSingleBBDTO>(projectFrom);
        }

        public static List<AggregateRootCompositeManyBCompositeSingleBBDTO> MapToAggregateRootCompositeManyBCompositeSingleBBDTOList(this IEnumerable<CompositeSingleBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToAggregateRootCompositeManyBCompositeSingleBBDTO(mapper)).ToList();
        }
    }
}