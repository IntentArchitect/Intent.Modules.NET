using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record OrderLineDto
    {
        public OrderLineDto()
        {
            ProductName = null!;
        }

        public Guid Id { get; init; }
        public string ProductName { get; init; }
        public int Qty { get; init; }
        public string? DiscountCode { get; init; }
        public decimal UnitPrice { get; init; }
    }
}