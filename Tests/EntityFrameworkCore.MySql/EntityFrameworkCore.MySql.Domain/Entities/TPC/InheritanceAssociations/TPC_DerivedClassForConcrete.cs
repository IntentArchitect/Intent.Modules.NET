using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForConcrete : TPC_ConcreteBaseClass
    {
        public TPC_DerivedClassForConcrete()
        {
            DerivedAttribute = null!;
        }
        public string DerivedAttribute { get; set; }
    }
}