using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForConcrete : TPH_ConcreteBaseClass
    {
        public TPH_DerivedClassForConcrete()
        {
            DerivedAttribute = null!;
        }
        public string DerivedAttribute { get; set; }
    }
}