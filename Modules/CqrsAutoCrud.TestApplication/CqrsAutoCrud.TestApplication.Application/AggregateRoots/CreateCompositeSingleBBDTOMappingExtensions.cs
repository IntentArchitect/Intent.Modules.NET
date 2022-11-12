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
    public static class CreateCompositeSingleBBDTOMappingExtensions
    {
        public static CreateCompositeSingleBBDTO MapToCreateCompositeSingleBBDTO(this CompositeSingleBB projectFrom, IMapper mapper)
        {
            return mapper.Map<CreateCompositeSingleBBDTO>(projectFrom);
        }

        public static List<CreateCompositeSingleBBDTO> MapToCreateCompositeSingleBBDTOList(this IEnumerable<CompositeSingleBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCreateCompositeSingleBBDTO(mapper)).ToList();
        }
    }
}