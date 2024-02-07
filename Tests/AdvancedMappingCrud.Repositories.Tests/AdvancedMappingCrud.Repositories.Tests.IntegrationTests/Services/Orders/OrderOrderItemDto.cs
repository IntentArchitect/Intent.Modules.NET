using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class OrderOrderItemDto
    {
        public Guid OrderId { get; set; }
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }

        public static OrderOrderItemDto Create(Guid orderId, Guid id, int quantity, decimal amount, Guid productId)
        {
            return new OrderOrderItemDto
            {
                OrderId = orderId,
                Id = id,
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }
    }
}