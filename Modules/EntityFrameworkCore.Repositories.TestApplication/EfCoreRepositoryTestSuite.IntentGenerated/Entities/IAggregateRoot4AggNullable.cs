using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial interface IAggregateRoot4AggNullable
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        AggregateRoot4Single AggregateRoot4Single { get; set; }

        ICollection<AggregateRoot4Collection> AggregateRoot4Collections { get; set; }

        AggregateRoot4Nullable AggregateRoot4Nullable { get; set; }

    }
}
