using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace HttpQuery.FastEndpoints.Domain.Products
{
    public class Product
    {
        public Product()
        {
            Name = null!;
            Category = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }
    }
}