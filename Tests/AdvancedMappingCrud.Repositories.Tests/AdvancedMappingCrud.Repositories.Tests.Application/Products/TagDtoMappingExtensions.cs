using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public static class TagDtoMappingExtensions
    {
        public static TagDto MapToTagDto(this Tag projectFrom, IMapper mapper)
            => mapper.Map<TagDto>(projectFrom);

        public static List<TagDto> MapToTagDtoList(this IEnumerable<Tag> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTagDto(mapper)).ToList();
    }
}