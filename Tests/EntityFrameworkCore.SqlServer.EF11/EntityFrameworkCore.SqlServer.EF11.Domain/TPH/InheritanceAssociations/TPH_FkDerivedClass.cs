using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPH.InheritanceAssociations
{
    public class TPH_FkDerivedClass : TPH_FkBaseClass
    {
        public TPH_FkDerivedClass()
        {
            DerivedField = null!;
        }

        public string DerivedField { get; set; }
    }
}