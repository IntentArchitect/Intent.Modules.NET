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
            DispatchDocumentNumber = null!;
            ManifestCarrierCode = null!;
            ManifestDocumentNumber = null!;
        }

        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string DispatchDocumentNumber { get; set; }
        public DateTime DispatchDocumentIssuedOn { get; set; }
        public string ManifestCarrierCode { get; set; }
        public decimal ManifestTotalWeight { get; set; }
        public string ManifestDocumentNumber { get; set; }
        public DateTime ManifestDocumentIssuedOn { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedOn { get; set; }

        public static OrderShipmentDto Create(
            Guid id, string? trackingNumber, DateTime? shippedOn, string provider, string dispatchDocumentNumber, DateTime dispatchDocumentIssuedOn, string manifestCarrierCode, decimal manifestTotalWeight, string manifestDocumentNumber, DateTime manifestDocumentIssuedOn)
        {
            return new OrderShipmentDto
            {
                Id = id,
                TrackingNumber = trackingNumber,
                ShippedOn = shippedOn
,
                Provider = provider,
                DispatchDocumentNumber = dispatchDocumentNumber,
                DispatchDocumentIssuedOn = dispatchDocumentIssuedOn,
                ManifestCarrierCode = manifestCarrierCode,
                ManifestTotalWeight = manifestTotalWeight,
                ManifestDocumentNumber = manifestDocumentNumber,
                ManifestDocumentIssuedOn = manifestDocumentIssuedOn
            };
        }
    }
}