using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    public class Poly_ConcreteB : Poly_BaseClassNonAbstract
    {
        public Poly_ConcreteB()
        {
            ConcreteField = null!;
        }

        public string ConcreteField { get; set; }
    }
}