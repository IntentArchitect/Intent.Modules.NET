using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.Polymorphic
{
    public class TPH_Poly_ConcreteB : TPH_Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}