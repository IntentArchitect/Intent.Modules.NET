using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders
{
    public class CreateOrderOrderLineDto
    {
        public CreateOrderOrderLineDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public decimal Amount { get; set; }

        public static CreateOrderOrderLineDto Create(string description, decimal amount)
        {
            return new CreateOrderOrderLineDto
            {
                Description = description,
                Amount = amount
            };
        }
    }
}