using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    public static class MultikeyIndexEntitySingleChildDtoMappingExtensions
    {
        public static MultikeyIndexEntitySingleChildDto MapToMultikeyIndexEntitySingleChildDto(this MultikeyIndexEntitySingleChild projectFrom, IMapper mapper)
            => mapper.Map<MultikeyIndexEntitySingleChildDto>(projectFrom);

        public static List<MultikeyIndexEntitySingleChildDto> MapToMultikeyIndexEntitySingleChildDtoList(this IEnumerable<MultikeyIndexEntitySingleChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultikeyIndexEntitySingleChildDto(mapper)).ToList();
    }
}