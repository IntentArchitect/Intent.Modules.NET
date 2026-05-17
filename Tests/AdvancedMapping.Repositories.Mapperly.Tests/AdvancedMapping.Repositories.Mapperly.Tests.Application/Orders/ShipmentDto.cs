using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class ShipmentDto
    {
        public ShipmentDto()
        {
            Provider = null!;
            DispatchOriginLocation = null!;
            DispatchDocumentNumber = null!;
            ManifestCarrierCode = null!;
            ManifestDocumentNumber = null!;
            CustomsDocuments = null!;
        }

        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Provider { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedOn { get; set; }
        public string DispatchOriginLocation { get; set; }
        public string DispatchDocumentNumber { get; set; }
        public DateTime DispatchDocumentIssuedOn { get; set; }
        public string ManifestCarrierCode { get; set; }
        public decimal ManifestTotalWeight { get; set; }
        public string ManifestDocumentNumber { get; set; }
        public DateTime ManifestDocumentIssuedOn { get; set; }
        public List<ShipmentCustomsDocumentDto> CustomsDocuments { get; set; }

        public static ShipmentDto Create(
            Guid id,
            Guid orderId,
            string provider,
            string? trackingNumber,
            DateTime? shippedOn,
            string dispatchOriginLocation,
            string dispatchDocumentNumber,
            DateTime dispatchDocumentIssuedOn,
            string manifestCarrierCode,
            decimal manifestTotalWeight,
            string manifestDocumentNumber,
            DateTime manifestDocumentIssuedOn, List<ShipmentCustomsDocumentDto> customsDocuments)
        {
            return new ShipmentDto
            {
                Id = id,
                OrderId = orderId,
                Provider = provider,
                TrackingNumber = trackingNumber,
                ShippedOn = shippedOn,
                DispatchOriginLocation = dispatchOriginLocation,
                DispatchDocumentNumber = dispatchDocumentNumber,
                DispatchDocumentIssuedOn = dispatchDocumentIssuedOn,
                ManifestCarrierCode = manifestCarrierCode,
                ManifestTotalWeight = manifestTotalWeight,
                ManifestDocumentNumber = manifestDocumentNumber,
                ManifestDocumentIssuedOn = manifestDocumentIssuedOn,
                CustomsDocuments = customsDocuments
            };
        }
    }
}