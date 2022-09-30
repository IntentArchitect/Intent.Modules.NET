using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial class ExplicitKeysCompositeKey : IExplicitKeysCompositeKey
    {

        public Guid CompositeKeyA { get; set; }

        public Guid CompositeKeyB { get; set; }


    }
}
