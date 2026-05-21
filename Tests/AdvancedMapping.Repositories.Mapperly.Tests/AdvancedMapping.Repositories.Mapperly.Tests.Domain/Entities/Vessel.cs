using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Vessel
    {
        public Vessel()
        {
            IMOCode = null!;
        }

        public Guid Id { get; set; }

        public Guid ContainerId { get; set; }

        public string IMOCode { get; set; }
    }
}