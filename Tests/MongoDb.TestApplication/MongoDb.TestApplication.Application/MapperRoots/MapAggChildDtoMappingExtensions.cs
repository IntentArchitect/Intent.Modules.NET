using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public static class MapAggChildDtoMappingExtensions
    {
        public static MapAggChildDto MapToMapAggChildDto(this MapAggChild projectFrom, IMapper mapper)
            => mapper.Map<MapAggChildDto>(projectFrom);

        public static List<MapAggChildDto> MapToMapAggChildDtoList(this IEnumerable<MapAggChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMapAggChildDto(mapper)).ToList();
    }
}