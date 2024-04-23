using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForAbstract : TPH_AbstractBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}