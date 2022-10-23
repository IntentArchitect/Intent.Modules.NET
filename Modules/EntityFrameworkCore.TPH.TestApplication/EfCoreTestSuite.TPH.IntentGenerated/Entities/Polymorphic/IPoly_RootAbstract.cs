using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public interface IPoly_RootAbstract
    {
        string AbstractField { get; set; }

        IPoly_RootAbstract_Aggr Poly_RootAbstract_Aggr { get; set; }

        IPoly_RootAbstract_Comp Poly_RootAbstract_Comp { get; set; }
    }
}
