using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_ConcreteB : TPC_Poly_BaseClassNonAbstract
    {
        public TPC_Poly_ConcreteB()
        {
            ConcreteField = null!;
        }
        public string ConcreteField { get; set; }
    }
}