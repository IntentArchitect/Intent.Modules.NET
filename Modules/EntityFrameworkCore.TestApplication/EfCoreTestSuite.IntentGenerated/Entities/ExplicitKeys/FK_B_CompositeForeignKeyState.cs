using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class FK_B_CompositeForeignKey : IFK_B_CompositeForeignKey
    {

        public Guid Id { get; set; }

        public Guid PKCompositeKeyCompositeKeyA { get; set; }

        public Guid PKCompositeKeyCompositeKeyB { get; set; }

        public virtual PK_B_CompositeKey PKCompositeKey { get; set; }

        IPK_B_CompositeKey IFK_B_CompositeForeignKey.PKCompositeKey
        {
            get => PKCompositeKey;
            set => PKCompositeKey = (PK_B_CompositeKey)value;
        }


    }
}
