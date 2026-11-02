using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.Products
{
    public record ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Category = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Category { get; init; }
        public decimal Price { get; init; }
    }
}