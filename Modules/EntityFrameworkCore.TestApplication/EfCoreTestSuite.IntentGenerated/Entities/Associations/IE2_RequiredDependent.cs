using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IE2_RequiredDependent
    {

        string ReqDepAttr { get; set; }

        IE2_RequiredCompositeNav E2RequiredCompositeNav { get; set; }

    }
}
