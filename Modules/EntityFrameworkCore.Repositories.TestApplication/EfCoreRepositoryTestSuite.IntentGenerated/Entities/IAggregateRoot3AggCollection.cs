using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public interface IAggregateRoot3AggCollection
    {

        IAggregateRoot3Single AggregateRoot3Single { get; set; }

        IAggregateRoot3Nullable AggregateRoot3Nullable { get; set; }

        ICollection<IAggregateRoot3Collection> AggregateRoot3Collections { get; set; }

    }
}
