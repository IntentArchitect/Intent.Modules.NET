using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteA : TPT_Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}