using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    public static class TextIndexEntityDtoMappingExtensions
    {
        public static TextIndexEntityDto MapToTextIndexEntityDto(this TextIndexEntity projectFrom, IMapper mapper)
            => mapper.Map<TextIndexEntityDto>(projectFrom);

        public static List<TextIndexEntityDto> MapToTextIndexEntityDtoList(this IEnumerable<TextIndexEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTextIndexEntityDto(mapper)).ToList();
    }
}