using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
            Name = null!;
            Tags = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UpdateProductTagDto> Tags { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, List<UpdateProductTagDto> tags)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }
    }
}