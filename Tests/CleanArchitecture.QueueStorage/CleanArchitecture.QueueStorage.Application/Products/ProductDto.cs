using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application.Products
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }

        public static ProductDto Create(Guid id, string name, int qty)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Qty = qty
            };
        }
    }
}