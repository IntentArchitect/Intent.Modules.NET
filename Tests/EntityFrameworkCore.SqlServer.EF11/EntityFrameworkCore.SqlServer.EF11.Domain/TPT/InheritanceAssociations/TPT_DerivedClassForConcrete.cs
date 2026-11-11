using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcrete : TPT_ConcreteBaseClass
    {
        public TPT_DerivedClassForConcrete()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
    }
}