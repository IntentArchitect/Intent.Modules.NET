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
    public static class A_AggregateRootDTOMappingExtensions
    {
        public static A_AggregateRootDTO MapToA_AggregateRootDTO(this IA_AggregateRoot projectFrom, IMapper mapper)
        {
            return mapper.Map<A_AggregateRootDTO>(projectFrom);
        }

        public static List<A_AggregateRootDTO> MapToA_AggregateRootDTOList(this IEnumerable<IA_AggregateRoot> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToA_AggregateRootDTO(mapper)).ToList();
        }
    }
}