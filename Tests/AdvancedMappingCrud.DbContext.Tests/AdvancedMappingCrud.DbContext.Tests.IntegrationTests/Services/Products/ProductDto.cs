using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public static ProductDto Create(Guid id, string name, decimal price)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Price = price
            };
        }
    }
}