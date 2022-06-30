using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial interface IAggregateRoot3AggCollection
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        Guid AggregateRoot3SingleId { get; }
        AggregateRoot3Single AggregateRoot3Single { get; set; }

        Guid? AggregateRoot3NullableId { get; }
        AggregateRoot3Nullable AggregateRoot3Nullable { get; set; }

        ICollection<AggregateRoot3Collection> AggregateRoot3Collections { get; set; }

    }
}
