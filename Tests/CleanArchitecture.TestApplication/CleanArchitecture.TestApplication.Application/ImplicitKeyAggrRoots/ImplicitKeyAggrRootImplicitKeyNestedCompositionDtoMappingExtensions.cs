using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    public static class ImplicitKeyAggrRootImplicitKeyNestedCompositionDtoMappingExtensions
    {
        public static ImplicitKeyAggrRootImplicitKeyNestedCompositionDto MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDto(this ImplicitKeyNestedComposition projectFrom, IMapper mapper)
        {
            return mapper.Map<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>(projectFrom);
        }

        public static List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDtoList(this IEnumerable<ImplicitKeyNestedComposition> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDto(mapper)).ToList();
        }
    }
}