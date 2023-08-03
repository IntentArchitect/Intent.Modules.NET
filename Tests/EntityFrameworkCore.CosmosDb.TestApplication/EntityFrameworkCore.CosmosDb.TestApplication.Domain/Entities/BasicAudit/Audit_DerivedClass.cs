using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Audit_DerivedClass : Audit_BaseClass
    {
        [IntentManaged(Mode.Fully)]
        public Audit_DerivedClass()
        {
            DerivedAttr = null!;
        }
        public string DerivedAttr { get; set; }
    }
}