using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IM_SelfReferenceBiNav
    {

        string PartitionKey { get; set; }

        string SelfRefBiNavAttr { get; set; }

        IM_SelfReferenceBiNav MSelfReferenceBiNavAssocation { get; set; }

        ICollection<IM_SelfReferenceBiNav> MSelfReferenceBiNavs { get; set; }
    }
}