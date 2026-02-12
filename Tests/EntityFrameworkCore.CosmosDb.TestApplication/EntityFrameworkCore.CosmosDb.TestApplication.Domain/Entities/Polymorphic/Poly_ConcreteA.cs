using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    public class Poly_ConcreteA : Poly_BaseClassNonAbstract, IHasDomainEvent
    {
        public Poly_ConcreteA()
        {
            ConcreteField = null!;
        }

        public string ConcreteField { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}