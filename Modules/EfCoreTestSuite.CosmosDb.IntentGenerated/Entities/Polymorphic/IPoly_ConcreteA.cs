using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public interface IPoly_ConcreteA : IPoly_BaseClassNonAbstract
    {
        string ConcreteField { get; set; }

        string PartitionKey { get; set; }

    }
}
