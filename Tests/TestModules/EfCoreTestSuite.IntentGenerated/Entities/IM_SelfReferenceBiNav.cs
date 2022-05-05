using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial interface IM_SelfReferenceBiNav
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        Guid? M_SelfReferenceBiNavDstId { get; }
        M_SelfReferenceBiNav M_SelfReferenceBiNavDst { get; set; }

        ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }

    }
}
