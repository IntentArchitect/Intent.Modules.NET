using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class StandaloneDerived : StandaloneAbstract
    {
        public StandaloneDerived()
        {
            DerivedAttribute = null!;
        }
        public string DerivedAttribute { get; set; }
    }
}