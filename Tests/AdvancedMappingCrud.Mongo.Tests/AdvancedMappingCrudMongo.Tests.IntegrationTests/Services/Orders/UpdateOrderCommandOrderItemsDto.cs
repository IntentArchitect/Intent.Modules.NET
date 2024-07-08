using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders
{
    public class UpdateOrderCommandOrderItemsDto
    {
        public UpdateOrderCommandOrderItemsDto()
        {
            Id = null!;
            ProductId = null!;
        }

        public string Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }

        public static UpdateOrderCommandOrderItemsDto Create(string id, int quantity, decimal amount, string productId)
        {
            return new UpdateOrderCommandOrderItemsDto
            {
                Id = id,
                Quantity = quantity,
                Amount = amount,
                ProductId = productId
            };
        }
    }
}