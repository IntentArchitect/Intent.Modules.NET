using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Shipment
    {
        public Shipment()
        {
            Provider = null!;
        }

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Provider { get; set; }

        public string? TrackingNumber { get; set; }

        public DateTime? ShippedOn { get; set; }
    }
}