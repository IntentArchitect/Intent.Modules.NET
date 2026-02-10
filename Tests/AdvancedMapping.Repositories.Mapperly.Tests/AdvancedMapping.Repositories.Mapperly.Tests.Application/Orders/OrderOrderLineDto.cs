using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderOrderLineDto
    {
        public OrderOrderLineDto()
        {
            Product = null!;
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? LineTotal { get; set; }
        public OrderProductDto Product { get; set; }

        public static OrderOrderLineDto Create(
            Guid id,
            Guid productId,
            int quantity,
            decimal unitPrice,
            decimal? lineTotal,
            OrderProductDto product)
        {
            return new OrderOrderLineDto
            {
                Id = id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal,
                Product = product
            };
        }
    }
}