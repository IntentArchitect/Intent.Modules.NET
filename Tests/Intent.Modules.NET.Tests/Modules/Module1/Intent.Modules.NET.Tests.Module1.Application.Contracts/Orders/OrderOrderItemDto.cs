using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders
{
    public class OrderOrderItemDto
    {
        public OrderOrderItemDto()
        {
        }

        public int Quantiity { get; set; }
        public decimal Amount { get; set; }
        public Guid Id { get; set; }

        public static OrderOrderItemDto Create(int quantiity, decimal amount, Guid id)
        {
            return new OrderOrderItemDto
            {
                Quantiity = quantiity,
                Amount = amount,
                Id = id
            };
        }
    }
}