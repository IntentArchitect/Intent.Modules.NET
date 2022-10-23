using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract_Aggr : IPoly_RootAbstract_Aggr, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AggrField { get; set; }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
