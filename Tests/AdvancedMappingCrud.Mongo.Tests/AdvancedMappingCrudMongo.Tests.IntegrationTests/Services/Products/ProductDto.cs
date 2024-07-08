using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Id = null!;
            Name = null!;
            Description = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static ProductDto Create(string id, string name, string description)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = description
            };
        }
    }
}