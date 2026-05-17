using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class CustomsDocument
    {
        public CustomsDocument()
        {
            DocumentNumber = null!;
        }

        public Guid Id { get; set; }

        public Guid CustomsId { get; set; }

        public string DocumentNumber { get; set; }

        public CustomsDocumentType DocumentType { get; set; }
    }
}