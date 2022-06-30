using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial interface IAggregateRoot2Composition
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        AggregateRoot2Single AggregateRoot2Single { get; set; }

        AggregateRoot2Nullable AggregateRoot2Nullable { get; set; }

        ICollection<AggregateRoot2Collection> AggregateRoot2Collections { get; set; }

    }
}
