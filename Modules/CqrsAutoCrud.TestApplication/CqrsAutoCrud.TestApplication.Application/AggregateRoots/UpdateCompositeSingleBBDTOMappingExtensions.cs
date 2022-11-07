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
    public static class UpdateCompositeSingleBBDTOMappingExtensions
    {
        public static UpdateCompositeSingleBBDTO MapToUpdateCompositeSingleBBDTO(this ICompositeSingleBB projectFrom, IMapper mapper)
        {
            return mapper.Map<UpdateCompositeSingleBBDTO>(projectFrom);
        }

        public static List<UpdateCompositeSingleBBDTO> MapToUpdateCompositeSingleBBDTOList(this IEnumerable<ICompositeSingleBB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToUpdateCompositeSingleBBDTO(mapper)).ToList();
        }
    }
}