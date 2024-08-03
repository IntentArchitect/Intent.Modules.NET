using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_FkDerivedClass : TPH_FkBaseClass
    {
        public string DerivedField { get; set; }
    }
}