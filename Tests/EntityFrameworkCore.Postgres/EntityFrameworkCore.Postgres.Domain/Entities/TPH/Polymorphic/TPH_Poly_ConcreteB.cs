using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic
{
    public class TPH_Poly_ConcreteB : TPH_Poly_BaseClassNonAbstract
    {
        public TPH_Poly_ConcreteB()
        {
            ConcreteField = null!;
        }
        public string ConcreteField { get; set; }
    }
}