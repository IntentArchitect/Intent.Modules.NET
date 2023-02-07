using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public interface IFK_A_CompositeForeignKey
    {

        IPK_A_CompositeKey PK_A_CompositeKey { get; set; }

    }
}
