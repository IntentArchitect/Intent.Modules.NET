using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Category = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        public static ProductDto Create(Guid id, string name, string category, decimal price)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Category = category,
                Price = price
            };
        }
    }
}