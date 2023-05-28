using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    public static class ImplicitKeyAggrRootImplicitKeyNestedCompositionDtoMappingExtensions
    {
        public static ImplicitKeyAggrRootImplicitKeyNestedCompositionDto MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDto(this ImplicitKeyNestedComposition projectFrom, IMapper mapper)
            => mapper.Map<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>(projectFrom);

        public static List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDtoList(this IEnumerable<ImplicitKeyNestedComposition> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDto(mapper)).ToList();
    }
}