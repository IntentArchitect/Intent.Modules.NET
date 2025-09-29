using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Blazor.ProductService.Application
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Ref = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public int Qty { get; set; }

        public static ProductDto Create(Guid id, string name, string @ref, int qty)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Ref = @ref,
                Qty = qty
            };
        }
    }
}