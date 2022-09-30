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

        public virtual PK_A_CompositeKey PK_A_CompositeKey { get; set; }

        IPK_A_CompositeKey IFK_A_CompositeForeignKey.PK_A_CompositeKey
        {
            get => PK_A_CompositeKey;
            set => PK_A_CompositeKey = (PK_A_CompositeKey)value;
        }

        public Guid PK_A_CompositeKeyCompositeKeyA { get; set; }

        public Guid PK_A_CompositeKeyCompositeKeyB { get; set; }


    }
}
