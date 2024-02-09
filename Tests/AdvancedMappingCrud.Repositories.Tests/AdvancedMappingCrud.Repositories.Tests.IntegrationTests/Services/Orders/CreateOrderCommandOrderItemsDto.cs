using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class CreateOrderCommandOrderItemsDto
    {
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }

        public static CreateOrderCommandOrderItemsDto Create(int quantity, decimal amount, Guid productId)
        {
            return new CreateOrderCommandOrderItemsDto
            {
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }
    }
}