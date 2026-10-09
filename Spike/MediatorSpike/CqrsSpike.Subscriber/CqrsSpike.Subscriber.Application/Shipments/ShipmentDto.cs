using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments
{
    public record ShipmentDto
    {
        public ShipmentDto()
        {
            Status = null!;
        }

        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public string Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}