using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcrete : TPT_ConcreteBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}