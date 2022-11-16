using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IH_MultipleDependent
    {

        string MultipleDepAttr { get; set; }

        IH_OptionalAggregateNav H_OptionalAggregateNav { get; set; }

    }
}
