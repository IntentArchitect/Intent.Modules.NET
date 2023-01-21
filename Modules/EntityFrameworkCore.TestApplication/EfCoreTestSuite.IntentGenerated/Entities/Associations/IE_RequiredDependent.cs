using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IE_RequiredDependent
    {

        string RequiredDepAttr { get; set; }

        IE_RequiredCompositeNav ERequiredCompositeNav { get; set; }

    }
}
