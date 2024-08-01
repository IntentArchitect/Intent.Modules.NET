using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForAbstract : TPT_AbstractBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}