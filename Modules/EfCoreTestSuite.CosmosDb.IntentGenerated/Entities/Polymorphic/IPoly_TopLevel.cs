using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public interface IPoly_TopLevel
    {
        string TopField { get; set; }

        string PartitionKey { get; set; }

        ICollection<IPoly_RootAbstract> Poly_RootAbstracts { get; set; }

    }
}
