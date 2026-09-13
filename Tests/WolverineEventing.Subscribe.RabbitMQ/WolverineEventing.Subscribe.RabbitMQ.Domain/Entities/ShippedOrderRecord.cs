using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Domain.Entities
{
    public class ShippedOrderRecord
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public DateTime ShippedAt { get; set; }
    }
}