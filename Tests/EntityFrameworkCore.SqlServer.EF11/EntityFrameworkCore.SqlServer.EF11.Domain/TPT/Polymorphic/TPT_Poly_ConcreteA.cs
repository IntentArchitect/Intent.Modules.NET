using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteA : TPT_Poly_BaseClassNonAbstract
    {
        public TPT_Poly_ConcreteA()
        {
            ConcreteField = null!;
        }

        public string ConcreteField { get; set; }
    }
}