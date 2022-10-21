using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_TopLevel : IPoly_TopLevel, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string TopField { get; set; }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
