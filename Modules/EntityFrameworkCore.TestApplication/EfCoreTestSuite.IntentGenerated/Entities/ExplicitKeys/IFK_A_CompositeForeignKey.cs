using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial interface IFK_A_CompositeForeignKey
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        Guid ForeignCompositeKeyA { get; set; }

        Guid ForeignCompositeKeyB { get; set; }

        PK_A_CompositeKey PK_CompositeKey { get; set; }

    }
}
