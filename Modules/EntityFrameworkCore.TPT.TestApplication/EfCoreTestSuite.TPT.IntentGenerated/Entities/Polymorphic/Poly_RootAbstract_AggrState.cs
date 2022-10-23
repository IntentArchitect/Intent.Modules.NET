using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract_Aggr : IPoly_RootAbstract_Aggr, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AggrField { get; set; }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
