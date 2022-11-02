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
    public static class CompositeSingleAAA2DTOMappingExtensions
    {
        public static CompositeSingleAAA2DTO MapToCompositeSingleAAA2DTO(this ICompositeSingleAAA2 projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeSingleAAA2DTO>(projectFrom);
        }

        public static List<CompositeSingleAAA2DTO> MapToCompositeSingleAAA2DTOList(this IEnumerable<ICompositeSingleAAA2> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeSingleAAA2DTO(mapper)).ToList();
        }
    }
}