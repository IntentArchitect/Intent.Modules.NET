using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForConcrete : TPC_ConcreteBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}