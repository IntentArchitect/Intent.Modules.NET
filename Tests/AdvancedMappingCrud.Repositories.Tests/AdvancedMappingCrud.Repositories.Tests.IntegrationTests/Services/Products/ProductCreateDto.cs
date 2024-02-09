using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
            Tags = null!;
        }

        public string Name { get; set; }
        public List<CreateProductTagDto> Tags { get; set; }

        public static ProductCreateDto Create(string name, List<CreateProductTagDto> tags)
        {
            return new ProductCreateDto
            {
                Name = name,
                Tags = tags
            };
        }
    }
}