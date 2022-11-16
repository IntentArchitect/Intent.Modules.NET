using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IF_OptionalAggregateNav
    {

        string OptionalAggrNavAttr { get; set; }

        IF_OptionalDependent F_OptionalDependent { get; set; }

    }
}
