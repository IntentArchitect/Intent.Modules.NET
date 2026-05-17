using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Customs
    {
        public Customs()
        {
            OriginCountry = null!;
            DestinationCountry = null!;
        }

        public Guid Id { get; set; }

        public string OriginCountry { get; set; }

        public string DestinationCountry { get; set; }

        public virtual ICollection<CustomsDocument> CustomsDocuments { get; set; } = [];
    }
}