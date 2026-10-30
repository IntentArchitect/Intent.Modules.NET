using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.Subscriber.Domain
{
    public class Shipment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
    }
}