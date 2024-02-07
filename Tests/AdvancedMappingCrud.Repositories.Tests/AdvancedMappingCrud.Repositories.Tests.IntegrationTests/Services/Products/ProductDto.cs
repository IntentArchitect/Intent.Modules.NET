using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Tags = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TagDto> Tags { get; set; }

        public static ProductDto Create(Guid id, string name, List<TagDto> tags)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }
    }
}