using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EnumStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    public static class RootEntityDtoMappingExtensions
    {
        public static RootEntityDto MapToRootEntityDto(this RootEntity projectFrom, IMapper mapper)
            => mapper.Map<RootEntityDto>(projectFrom);

        public static List<RootEntityDto> MapToRootEntityDtoList(this IEnumerable<RootEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToRootEntityDto(mapper)).ToList();
    }
}