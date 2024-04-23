using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.Polymorphic
{
    public class TPC_Poly_ConcreteB : TPC_Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}