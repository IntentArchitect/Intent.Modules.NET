using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application
{
    public record ShippedOrderRecordDto
    {
        public ShippedOrderRecordDto()
        {
        }

        public Guid OrderId { get; init; }
        public DateTime ShippedAt { get; init; }
    }
}