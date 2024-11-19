using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_ConcreteA : TPC_Poly_BaseClassNonAbstract
    {
        public TPC_Poly_ConcreteA()
        {
            ConcreteField = null!;
        }
        public string ConcreteField { get; set; }
    }
}