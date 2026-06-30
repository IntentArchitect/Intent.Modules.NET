using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public static class TagDtoMappingExtensions
    {
        public static TagDto MapToTagDto(this Tag projectFrom)
        {
            return new TagDto
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name
            };
        }

        public static List<TagDto> MapToTagDtoList(this IEnumerable<Tag> projectFrom) => projectFrom.Select(x => x.MapToTagDto()).ToList();
    }
}