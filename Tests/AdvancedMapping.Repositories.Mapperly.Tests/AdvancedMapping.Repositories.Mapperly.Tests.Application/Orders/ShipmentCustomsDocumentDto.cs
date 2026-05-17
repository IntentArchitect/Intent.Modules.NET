using AdvancedMapping.Repositories.Mapperly.Tests.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class ShipmentCustomsDocumentDto
    {
        public ShipmentCustomsDocumentDto()
        {
            DocumentNumber = null!;
        }

        public Guid Id { get; set; }
        public string DocumentNumber { get; set; }
        public CustomsDocumentType DocumentType { get; set; }

        public static ShipmentCustomsDocumentDto Create(Guid id, string documentNumber, CustomsDocumentType documentType)
        {
            return new ShipmentCustomsDocumentDto
            {
                Id = id,
                DocumentNumber = documentNumber,
                DocumentType = documentType
            };
        }
    }
}