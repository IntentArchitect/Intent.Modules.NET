using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPT.Polymorphic
{
    public class TPT_Poly_ConcreteB : TPT_Poly_BaseClassNonAbstract
    {
        public string ConcreteField { get; set; }
    }
}