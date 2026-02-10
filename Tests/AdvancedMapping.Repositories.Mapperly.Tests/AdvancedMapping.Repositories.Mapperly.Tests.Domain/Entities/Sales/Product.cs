using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Product
    {
        public Product()
        {
            Name = null!;
            Sku = null!;
            Category = null!;
        }

        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public decimal Price { get; set; }

        public float? WeightKg { get; set; }

        public virtual ProductCategory Category { get; set; }
    }
}