using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    public static class TextIndexEntityMultiChildDtoMappingExtensions
    {
        public static TextIndexEntityMultiChildDto MapToTextIndexEntityMultiChildDto(this TextIndexEntityMultiChild projectFrom, IMapper mapper)
            => mapper.Map<TextIndexEntityMultiChildDto>(projectFrom);

        public static List<TextIndexEntityMultiChildDto> MapToTextIndexEntityMultiChildDtoList(this IEnumerable<TextIndexEntityMultiChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTextIndexEntityMultiChildDto(mapper)).ToList();
    }
}