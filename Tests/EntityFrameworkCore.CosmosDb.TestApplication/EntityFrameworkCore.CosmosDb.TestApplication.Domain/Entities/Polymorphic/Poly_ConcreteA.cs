using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    public class Poly_ConcreteA : Poly_BaseClassNonAbstract
    {
        public Poly_ConcreteA()
        {
            ConcreteField = null!;
        }

        public string ConcreteField { get; set; }
    }
}