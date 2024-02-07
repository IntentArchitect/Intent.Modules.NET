using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderOrderItemCommand
    {
        public CreateOrderOrderItemCommand()
        {
            OrderId = null!;
            ProductId = null!;
        }

        public string OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }

        public static CreateOrderOrderItemCommand Create(string orderId, int quantity, decimal amount, string productId)
        {
            return new CreateOrderOrderItemCommand
            {
                OrderId = orderId,
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }
    }
}