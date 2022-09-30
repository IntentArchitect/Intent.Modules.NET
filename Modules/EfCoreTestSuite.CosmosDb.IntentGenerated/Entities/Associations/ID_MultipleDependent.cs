using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public interface ID_MultipleDependent
    {
        string MultipleDependentAttr { get; set; }

        string PartitionKey { get; set; }
    }
}
