using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_FkDerivedClass : TPT_FkBaseClass
    {
        public string DerivedField { get; set; }
    }
}