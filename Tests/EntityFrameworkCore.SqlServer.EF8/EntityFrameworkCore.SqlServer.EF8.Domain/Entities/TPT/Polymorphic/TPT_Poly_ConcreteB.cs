using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteB : TPT_Poly_BaseClassNonAbstract
    {
        public TPT_Poly_ConcreteB()
        {
            ConcreteField = null!;
        }
        public string ConcreteField { get; set; }
    }
}