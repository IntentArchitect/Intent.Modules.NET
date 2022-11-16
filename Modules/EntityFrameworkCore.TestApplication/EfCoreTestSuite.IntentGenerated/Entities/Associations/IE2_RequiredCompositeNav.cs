using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IE2_RequiredCompositeNav
    {

        string ReqCompNavAttr { get; set; }

        IE2_RequiredDependent E2_RequiredDependent { get; set; }

    }
}
