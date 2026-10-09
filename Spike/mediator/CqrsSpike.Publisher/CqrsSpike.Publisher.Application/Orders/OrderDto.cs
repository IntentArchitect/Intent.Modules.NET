using CqrsSpike.Publisher.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders
{
    public record OrderDto
    {
        public OrderDto()
        {
            CustomerName = null!;
        }

        public Guid Id { get; init; }
        public string CustomerName { get; init; }
        public decimal Total { get; init; }
        public OrderStatus Status { get; init; }
        public Guid? ShipmentId { get; init; }
    }
}