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
    public static class CompositeSingleAAA1DTOMappingExtensions
    {
        public static CompositeSingleAAA1DTO MapToCompositeSingleAAA1DTO(this ICompositeSingleAAA1 projectFrom, IMapper mapper)
        {
            return mapper.Map<CompositeSingleAAA1DTO>(projectFrom);
        }

        public static List<CompositeSingleAAA1DTO> MapToCompositeSingleAAA1DTOList(this IEnumerable<ICompositeSingleAAA1> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCompositeSingleAAA1DTO(mapper)).ToList();
        }
    }
}