using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class CreateOrderCommandManifestDto
    {
        public CreateOrderCommandManifestDto()
        {
            CarrierCode = null!;
            Document = null!;
        }

        public string CarrierCode { get; set; }
        public decimal TotalWeight { get; set; }
        public CreateOrderCommandManifestDocumentDto Document { get; set; }

        public static CreateOrderCommandManifestDto Create(
            string carrierCode,
            decimal totalWeight,
            CreateOrderCommandManifestDocumentDto document)
        {
            return new CreateOrderCommandManifestDto
            {
                CarrierCode = carrierCode,
                TotalWeight = totalWeight,
                Document = document
            };
        }
    }
}