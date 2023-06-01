using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public static class MapperRootDtoMappingExtensions
    {
        public static MapperRootDto MapToMapperRootDto(this MapperRoot projectFrom, IMapper mapper)
            => mapper.Map<MapperRootDto>(projectFrom);

        public static List<MapperRootDto> MapToMapperRootDtoList(this IEnumerable<MapperRoot> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMapperRootDto(mapper)).ToList();
    }
}