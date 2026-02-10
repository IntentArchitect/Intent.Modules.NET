using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderProductDto
    {
        public OrderProductDto()
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
        public OrderProductCategoryDto Category { get; set; }

        public static OrderProductDto Create(
            Guid id,
            Guid categoryId,
            string name,
            string sku,
            decimal price,
            float? weightKg,
            OrderProductCategoryDto category)
        {
            return new OrderProductDto
            {
                Id = id,
                CategoryId = categoryId,
                Name = name,
                Sku = sku,
                Price = price,
                WeightKg = weightKg,
                Category = category
            };
        }
    }
}