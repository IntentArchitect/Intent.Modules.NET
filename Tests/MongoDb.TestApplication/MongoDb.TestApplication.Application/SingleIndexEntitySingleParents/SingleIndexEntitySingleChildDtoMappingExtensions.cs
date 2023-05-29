using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    public static class SingleIndexEntitySingleChildDtoMappingExtensions
    {
        public static SingleIndexEntitySingleChildDto MapToSingleIndexEntitySingleChildDto(this SingleIndexEntitySingleChild projectFrom, IMapper mapper)
            => mapper.Map<SingleIndexEntitySingleChildDto>(projectFrom);

        public static List<SingleIndexEntitySingleChildDto> MapToSingleIndexEntitySingleChildDtoList(this IEnumerable<SingleIndexEntitySingleChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSingleIndexEntitySingleChildDto(mapper)).ToList();
    }
}