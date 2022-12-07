using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots
{
    public static class ImplicitKeyAggrRootDTOMappingExtensions
    {
        public static ImplicitKeyAggrRootDTO MapToImplicitKeyAggrRootDTO(this ImplicitKeyAggrRoot projectFrom, IMapper mapper)
        {
            return mapper.Map<ImplicitKeyAggrRootDTO>(projectFrom);
        }

        public static List<ImplicitKeyAggrRootDTO> MapToImplicitKeyAggrRootDTOList(this IEnumerable<ImplicitKeyAggrRoot> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToImplicitKeyAggrRootDTO(mapper)).ToList();
        }
    }
}