using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestMediator.Subscriber.Application.Shipments
{
    public record ShipmentDto
    {
        public ShipmentDto()
        {
        }

        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
    }
}