using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Products
{
    public static class DigitalProductDtoMappingExtensions
    {
        public static DigitalProductDto MapToDigitalProductDto(this DigitalProduct projectFrom)
        {
            return new DigitalProductDto
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Price = projectFrom.Price,
                DownloadUrl = projectFrom.DownloadUrl
            };
        }

        public static List<DigitalProductDto> MapToDigitalProductDtoList(this IEnumerable<DigitalProduct> projectFrom) => projectFrom.Select(x => x.MapToDigitalProductDto()).ToList();
    }
}