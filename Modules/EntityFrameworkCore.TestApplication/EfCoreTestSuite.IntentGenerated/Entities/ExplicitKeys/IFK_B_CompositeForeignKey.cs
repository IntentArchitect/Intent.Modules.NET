using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public interface IFK_B_CompositeForeignKey
    {

        Guid PK_CompositeKeyCompositeKeyA { get; set; }

        Guid PK_CompositeKeyCompositeKeyB { get; set; }

        IPK_B_CompositeKey PK_CompositeKey { get; set; }

    }
}
