using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderOrderItemCommand
    {
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public int Units { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderOrderItemCommand Create(Guid orderId, int quantity, decimal amount, int units, Guid productId)
        {
            return new CreateOrderOrderItemCommand
            {
                OrderId = orderId,
                Quantity = quantity,
                Amount = amount,
                Units = units,
                ProductId = productId
            };
        }
    }
}