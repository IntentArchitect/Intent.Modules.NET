using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public interface IA_RequiredComposite
    {
        string RequiredCompositeAttr { get; set; }

        string PartitionKey { get; set; }

        IA_OptionalDependent A_OptionalDependent { get; set; }

    }
}
