using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial interface IFK_ExplicitKeys_CompositeForeignKey
    {

        Guid Id { get; set; }

        Guid PkExplicitkeysCompositekeyCompositeKeyA { get; set; }

        Guid PkExplicitkeysCompositekeyCompositeKeyB { get; set; }
        Guid PK_ExplicitKeys_CompositeKeyCompositeKeyA { get; }
        Guid PK_ExplicitKeys_CompositeKeyCompositeKeyB { get; }
        PK_ExplicitKeys_CompositeKey PK_ExplicitKeys_CompositeKey { get; set; }

    }
}
