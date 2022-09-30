using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public interface IAggregateRoot4AggNullable
    {
        IAggregateRoot4Single AggregateRoot4Single { get; set; }

        ICollection<IAggregateRoot4Collection> AggregateRoot4Collections { get; set; }

        IAggregateRoot4Nullable AggregateRoot4Nullable { get; set; }

    }
}
