using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntitySingleParents
{
    public static class TextIndexEntitySingleChildDtoMappingExtensions
    {
        public static TextIndexEntitySingleChildDto MapToTextIndexEntitySingleChildDto(this TextIndexEntitySingleChild projectFrom, IMapper mapper)
            => mapper.Map<TextIndexEntitySingleChildDto>(projectFrom);

        public static List<TextIndexEntitySingleChildDto> MapToTextIndexEntitySingleChildDtoList(this IEnumerable<TextIndexEntitySingleChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTextIndexEntitySingleChildDto(mapper)).ToList();
    }
}