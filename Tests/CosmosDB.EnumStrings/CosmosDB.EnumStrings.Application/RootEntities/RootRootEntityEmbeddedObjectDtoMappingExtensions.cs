using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public static class RootRootEntityEmbeddedObjectDtoMappingExtensions
    {
        public static RootRootEntityEmbeddedObjectDto MapToRootRootEntityEmbeddedObjectDto(this EmbeddedObject projectFrom, IMapper mapper)
            => mapper.Map<RootRootEntityEmbeddedObjectDto>(projectFrom);

        public static List<RootRootEntityEmbeddedObjectDto> MapToRootRootEntityEmbeddedObjectDtoList(this IEnumerable<EmbeddedObject> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRootRootEntityEmbeddedObjectDto(mapper)).ToList();
    }
}