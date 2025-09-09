using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_FkDerivedClass : TPC_FkBaseClass
    {
        public TPC_FkDerivedClass()
        {
            DerivedField = null!;
        }

        public string DerivedField { get; set; }
    }
}