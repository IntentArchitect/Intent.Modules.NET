using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CqrsSpike.Subscriber.Domain.Entities
{
    public class Shipment
    {
        public Shipment()
        {
            Status = null!;
        }

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}