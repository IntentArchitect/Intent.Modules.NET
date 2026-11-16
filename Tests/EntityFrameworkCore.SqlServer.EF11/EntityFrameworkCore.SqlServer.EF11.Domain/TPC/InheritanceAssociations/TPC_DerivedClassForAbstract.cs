using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForAbstract : TPC_AbstractBaseClass
    {
        public TPC_DerivedClassForAbstract()
        {
            DerivedAttribute = null!;
        }

        public Guid Id { get; set; }

        public string DerivedAttribute { get; set; }
    }
}