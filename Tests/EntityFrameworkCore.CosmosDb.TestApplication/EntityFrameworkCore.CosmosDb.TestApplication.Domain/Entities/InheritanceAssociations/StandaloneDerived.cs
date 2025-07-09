using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public class StandaloneDerived : StandaloneAbstract
    {
        public StandaloneDerived()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
    }
}