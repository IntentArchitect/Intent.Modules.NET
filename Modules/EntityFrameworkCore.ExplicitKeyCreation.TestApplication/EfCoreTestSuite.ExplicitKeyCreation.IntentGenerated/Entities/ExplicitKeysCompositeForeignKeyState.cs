using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial class ExplicitKeysCompositeForeignKey : IExplicitKeysCompositeForeignKey
    {

        public Guid Id { get; set; }

        public Guid ExplicitKeysCompositeKeyCompositeKeyA { get; set; }

        public Guid ExplicitKeysCompositeKeyCompositeKeyB { get; set; }

        public virtual ExplicitKeysCompositeKey ExplicitKeysCompositeKey { get; set; }

        IExplicitKeysCompositeKey IExplicitKeysCompositeForeignKey.ExplicitKeysCompositeKey
        {
            get => ExplicitKeysCompositeKey;
            set => ExplicitKeysCompositeKey = (ExplicitKeysCompositeKey)value;
        }


    }
}
