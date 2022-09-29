using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial interface IM_SelfReferenceBiNav
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string PartitionKey { get; set; }

        string SelfRefBiNavAttr { get; set; }

        Guid? M_SelfReferenceBiNavAssocationId { get; }
        M_SelfReferenceBiNav M_SelfReferenceBiNavAssocation { get; set; }

        ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }

    }
}
