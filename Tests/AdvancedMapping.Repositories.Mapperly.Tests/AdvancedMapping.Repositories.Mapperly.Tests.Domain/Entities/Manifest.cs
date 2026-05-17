using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Manifest
    {
        public Manifest()
        {
            CarrierCode = null!;
            Document = null!;
        }

        public Guid Id { get; set; }

        public string CarrierCode { get; set; }

        public decimal TotalWeight { get; set; }

        public virtual ManifestDocument Document { get; set; }
    }
}