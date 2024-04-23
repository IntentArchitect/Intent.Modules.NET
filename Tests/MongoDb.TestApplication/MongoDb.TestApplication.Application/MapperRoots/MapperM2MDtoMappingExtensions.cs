using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public static class MapperM2MDtoMappingExtensions
    {
        public static MapperM2MDto MapToMapperM2MDto(this MapperM2M projectFrom, IMapper mapper)
            => mapper.Map<MapperM2MDto>(projectFrom);

        public static List<MapperM2MDto> MapToMapperM2MDtoList(this IEnumerable<MapperM2M> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMapperM2MDto(mapper)).ToList();
    }
}