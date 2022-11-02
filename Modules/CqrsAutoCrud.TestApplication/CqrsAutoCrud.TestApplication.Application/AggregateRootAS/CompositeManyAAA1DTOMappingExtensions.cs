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
    public static class CompositeManyAAA1DTOMappingExtensions
    {
        public static CompositeManyAAA1DTO MapToCompositeManyAAA1DTO(this ICompositeManyAAA1 projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeManyAAA1DTO>(projectFrom);
        }

        public static List<CompositeManyAAA1DTO> MapToCompositeManyAAA1DTOList(this IEnumerable<ICompositeManyAAA1> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeManyAAA1DTO(mapper)).ToList();
        }
    }
}