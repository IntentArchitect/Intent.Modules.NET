using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial interface IG_MultipleDependent
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string MultipleDepAttr { get; set; }

        Guid G_RequiredCompositeNavId { get; }
        G_RequiredCompositeNav G_RequiredCompositeNav { get; set; }

    }
}
