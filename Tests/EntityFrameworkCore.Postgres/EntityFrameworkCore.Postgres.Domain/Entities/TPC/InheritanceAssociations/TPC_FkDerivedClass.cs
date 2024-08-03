using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_FkDerivedClass : TPC_FkBaseClass
    {
        public string DerivedField { get; set; }
    }
}