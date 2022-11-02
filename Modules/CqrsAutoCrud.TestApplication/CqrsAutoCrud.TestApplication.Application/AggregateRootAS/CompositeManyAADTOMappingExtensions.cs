using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS
{
    public static class CompositeManyAADTOMappingExtensions
    {
        public static CompositeManyAADTO MapToCompositeManyAADTO(this ICompositeManyAA projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeManyAADTO>(projectFrom);
        }

        public static List<CompositeManyAADTO> MapToCompositeManyAADTOList(this IEnumerable<ICompositeManyAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeManyAADTO(mapper)).ToList();
        }
    }
}