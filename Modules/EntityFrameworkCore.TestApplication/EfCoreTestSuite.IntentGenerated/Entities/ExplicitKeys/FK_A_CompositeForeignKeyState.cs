using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class FK_A_CompositeForeignKey : IFK_A_CompositeForeignKey
    {

        public Guid Id { get; set; }

        public virtual PK_A_CompositeKey PKACompositeKey { get; set; }

        IPK_A_CompositeKey IFK_A_CompositeForeignKey.PKACompositeKey
        {
            get => PKACompositeKey;
            set => PKACompositeKey = (PK_A_CompositeKey)value;
        }

        public Guid PKACompositeKeyCompositeKeyA { get; set; }

        public Guid PKACompositeKeyCompositeKeyB { get; set; }


    }
}
