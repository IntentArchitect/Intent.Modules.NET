using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_ConcreteB : TPC_Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}