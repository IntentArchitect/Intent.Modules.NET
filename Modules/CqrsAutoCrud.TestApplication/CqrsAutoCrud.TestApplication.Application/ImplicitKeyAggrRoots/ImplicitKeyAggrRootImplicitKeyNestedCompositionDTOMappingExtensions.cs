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
    public static class ImplicitKeyAggrRootImplicitKeyNestedCompositionDTOMappingExtensions
    {
        public static ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDTO(this ImplicitKeyNestedComposition projectFrom, IMapper mapper)
        {
            return mapper.Map<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO>(projectFrom);
        }

        public static List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDTOList(this IEnumerable<ImplicitKeyNestedComposition> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDTO(mapper)).ToList();
        }
    }
}