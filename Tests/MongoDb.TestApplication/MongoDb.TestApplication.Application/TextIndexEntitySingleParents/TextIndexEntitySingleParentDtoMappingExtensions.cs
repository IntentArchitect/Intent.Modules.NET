using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntitySingleParents
{
    public static class TextIndexEntitySingleParentDtoMappingExtensions
    {
        public static TextIndexEntitySingleParentDto MapToTextIndexEntitySingleParentDto(this TextIndexEntitySingleParent projectFrom, IMapper mapper)
            => mapper.Map<TextIndexEntitySingleParentDto>(projectFrom);

        public static List<TextIndexEntitySingleParentDto> MapToTextIndexEntitySingleParentDtoList(this IEnumerable<TextIndexEntitySingleParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTextIndexEntitySingleParentDto(mapper)).ToList();
    }
}