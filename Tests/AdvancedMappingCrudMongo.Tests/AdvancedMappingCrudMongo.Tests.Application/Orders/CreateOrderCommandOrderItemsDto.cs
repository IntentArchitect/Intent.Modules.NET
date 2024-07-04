using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public class CreateOrderCommandOrderItemsDto
    {
        public CreateOrderCommandOrderItemsDto()
        {
            ProductId = null!;
        }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }

        public static CreateOrderCommandOrderItemsDto Create(int quantity, decimal amount, string productId)
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