using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_FkDerivedClass : TPT_FkBaseClass
    {
        public TPT_FkDerivedClass()
        {
            DerivedField = null!;
        }
        public string DerivedField { get; set; }
    }
}