using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public static class RootEntityNestedEntityEmbeddedObjectDtoMappingExtensions
    {
        public static RootEntityNestedEntityEmbeddedObjectDto MapToRootEntityNestedEntityEmbeddedObjectDto(this EmbeddedObject projectFrom, IMapper mapper)
            => mapper.Map<RootEntityNestedEntityEmbeddedObjectDto>(projectFrom);

        public static List<RootEntityNestedEntityEmbeddedObjectDto> MapToRootEntityNestedEntityEmbeddedObjectDtoList(this IEnumerable<EmbeddedObject> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRootEntityNestedEntityEmbeddedObjectDto(mapper)).ToList();
    }
}