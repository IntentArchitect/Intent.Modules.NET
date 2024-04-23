using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForConcrete : TPH_ConcreteBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}