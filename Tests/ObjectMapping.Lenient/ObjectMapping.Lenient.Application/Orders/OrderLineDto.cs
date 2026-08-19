using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public record OrderLineDto
    {
        public OrderLineDto()
        {
            ProductName = null!;
        }

        public Guid Id { get; init; }
        public string ProductName { get; init; }
        public int Quantity { get; init; }
    }
}