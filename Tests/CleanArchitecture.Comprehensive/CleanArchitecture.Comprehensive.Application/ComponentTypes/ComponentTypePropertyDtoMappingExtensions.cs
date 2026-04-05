using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes
{
    public static class ComponentTypePropertyDtoMappingExtensions
    {
        public static ComponentTypePropertyDto MapToComponentTypePropertyDto(this ComponentTypeProperty projectFrom, IMapper mapper)
            => mapper.Map<ComponentTypePropertyDto>(projectFrom);

        public static List<ComponentTypePropertyDto> MapToComponentTypePropertyDtoList(this IEnumerable<ComponentTypeProperty> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToComponentTypePropertyDto(mapper)).ToList();
    }
}