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
    public static class CompositeManyAAA2DTOMappingExtensions
    {
        public static CompositeManyAAA2DTO MapToCompositeManyAAA2DTO(this ICompositeManyAAA2 projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeManyAAA2DTO>(projectFrom);
        }

        public static List<CompositeManyAAA2DTO> MapToCompositeManyAAA2DTOList(this IEnumerable<ICompositeManyAAA2> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeManyAAA2DTO(mapper)).ToList();
        }
    }
}