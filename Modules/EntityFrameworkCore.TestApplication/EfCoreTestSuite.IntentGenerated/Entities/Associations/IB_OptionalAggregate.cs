using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IB_OptionalAggregate
    {

        string OptionalAggrAttr { get; set; }

        IB_OptionalDependent B_OptionalDependent { get; set; }

    }
}
