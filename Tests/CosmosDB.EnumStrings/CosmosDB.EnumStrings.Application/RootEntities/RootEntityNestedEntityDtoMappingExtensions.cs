using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EnumStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public static class RootEntityNestedEntityDtoMappingExtensions
    {
        public static RootEntityNestedEntityDto MapToRootEntityNestedEntityDto(this NestedEntity projectFrom, IMapper mapper)
            => mapper.Map<RootEntityNestedEntityDto>(projectFrom);

        public static List<RootEntityNestedEntityDto> MapToRootEntityNestedEntityDtoList(this IEnumerable<NestedEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRootEntityNestedEntityDto(mapper)).ToList();
    }
}