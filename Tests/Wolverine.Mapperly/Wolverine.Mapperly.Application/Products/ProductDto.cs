using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Wolverine.Mapperly.Application.Products
{
    /// <summary>
    /// Read model projected from the Product entity via Mapperly.
    /// </summary>
    public record ProductDto
    {
        public ProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
    }
}