using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    public static class TextIndexEntityMultiParentDtoMappingExtensions
    {
        public static TextIndexEntityMultiParentDto MapToTextIndexEntityMultiParentDto(this TextIndexEntityMultiParent projectFrom, IMapper mapper)
            => mapper.Map<TextIndexEntityMultiParentDto>(projectFrom);

        public static List<TextIndexEntityMultiParentDto> MapToTextIndexEntityMultiParentDtoList(this IEnumerable<TextIndexEntityMultiParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTextIndexEntityMultiParentDto(mapper)).ToList();
    }
}