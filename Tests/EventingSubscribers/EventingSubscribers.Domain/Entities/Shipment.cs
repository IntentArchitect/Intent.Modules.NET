using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Update Entity with VO destination address
    /// </summary>
    public class Shipment
    {
        public Shipment()
        {
            Title = null!;
            DestinationAddress = null!;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public Address DestinationAddress { get; set; }
    }
}