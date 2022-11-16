using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public interface IAggregateRoot2Composition
    {

        IAggregateRoot2Single AggregateRoot2Single { get; set; }

        IAggregateRoot2Nullable AggregateRoot2Nullable { get; set; }

        ICollection<IAggregateRoot2Collection> AggregateRoot2Collections { get; set; }

    }
}
