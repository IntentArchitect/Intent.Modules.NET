using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts
{
    public class CreateOrderOrderItemsDto
    {
        public CreateOrderOrderItemsDto()
        {
        }

        public int Quantiity { get; set; }
        public decimal Amount { get; set; }

        public static CreateOrderOrderItemsDto Create(int quantiity, decimal amount)
        {
            return new CreateOrderOrderItemsDto
            {
                Quantiity = quantiity,
                Amount = amount
            };
        }
    }
}