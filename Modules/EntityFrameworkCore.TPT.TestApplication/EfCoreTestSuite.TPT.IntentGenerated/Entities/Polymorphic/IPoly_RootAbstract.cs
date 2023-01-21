using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public interface IPoly_RootAbstract : IHasDomainEvent
    {

        string AbstractField { get; set; }

        IPoly_RootAbstract_Aggr PolyRootAbstractAggr { get; set; }

        IPoly_RootAbstract_Comp PolyRootAbstractComp { get; set; }
    }
}
