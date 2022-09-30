using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IH_OptionalAggregateNav
    {
        string OptionalAggrNavAttr { get; set; }

        ICollection<IH_MultipleDependent> H_MultipleDependents { get; set; }

    }
}
