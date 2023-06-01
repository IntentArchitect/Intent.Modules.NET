using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public static class MapMapMeDtoMappingExtensions
    {
        public static MapMapMeDto MapToMapMapMeDto(this MapMapMe projectFrom, IMapper mapper)
            => mapper.Map<MapMapMeDto>(projectFrom);

        public static List<MapMapMeDto> MapToMapMapMeDtoList(this IEnumerable<MapMapMe> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMapMapMeDto(mapper)).ToList();
    }
}