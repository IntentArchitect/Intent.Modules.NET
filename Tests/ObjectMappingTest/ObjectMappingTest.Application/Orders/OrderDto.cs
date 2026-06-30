using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record OrderDto
    {
        public OrderDto()
        {
            RefNo = null!;
            OrderLines = null!;
        }

        public Guid Id { get; init; }
        public string RefNo { get; init; }
        public Guid CustomerId { get; init; }
        public List<OrderLineDto> OrderLines { get; init; }
    }
}