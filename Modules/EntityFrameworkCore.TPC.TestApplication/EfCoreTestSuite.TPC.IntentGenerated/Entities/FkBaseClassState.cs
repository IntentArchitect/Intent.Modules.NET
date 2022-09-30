using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class FkBaseClass : IFkBaseClass
    {

        public Guid CompositeKeyA { get; set; }

        public Guid CompositeKeyB { get; set; }

    }
}
