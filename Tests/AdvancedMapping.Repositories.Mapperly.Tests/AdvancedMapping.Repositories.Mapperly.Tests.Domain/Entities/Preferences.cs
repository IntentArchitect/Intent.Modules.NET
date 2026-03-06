using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Preferences
    {
        public Guid Id { get; set; }

        public bool Newsletter { get; set; }

        public bool Specials { get; set; }
    }
}