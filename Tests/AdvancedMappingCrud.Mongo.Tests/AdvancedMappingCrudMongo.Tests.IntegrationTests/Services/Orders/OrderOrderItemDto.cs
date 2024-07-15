using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class OrderOrderItemDto
    {
        public OrderOrderItemDto()
        {
            ProductId = null!;
            Id = null!;
            Product = null!;
        }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public string Id { get; set; }
        public OrderOrderItemProductDto Product { get; set; }

        public static OrderOrderItemDto Create(
            int quantity,
            decimal amount,
            string productId,
            string id,
            OrderOrderItemProductDto product)
        {
            return new OrderOrderItemDto
            {
                Quantity = quantity,
                Amount = amount,
                ProductId = productId,
                Id = id,
                Product = product
            };
        }
    }
}