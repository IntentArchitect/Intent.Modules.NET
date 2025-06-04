using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public static ProductDto Create(Guid id, string name, string description, decimal price, string? imageUrl)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl
            };
        }
    }
}