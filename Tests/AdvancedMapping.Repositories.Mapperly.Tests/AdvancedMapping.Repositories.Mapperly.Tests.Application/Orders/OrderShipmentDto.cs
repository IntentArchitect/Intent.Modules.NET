using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderShipmentDto
    {
        public OrderShipmentDto()
        {
            Provider = null!;
        }

        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedOn { get; set; }

        public static OrderShipmentDto Create(Guid id, string provider, string? trackingNumber, DateTime? shippedOn)
        {
            return new OrderShipmentDto
            {
                Id = id,
                Provider = provider,
                TrackingNumber = trackingNumber,
                ShippedOn = shippedOn
            };
        }
    }
}