using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial interface IPoly_BaseClassNonAbstract : IPoly_RootAbstract
    {

        string BaseField { get; set; }

        Guid? Poly_SecondLevelId { get; }
    }
}
