using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class PK_PrimaryKeyInt : IPK_PrimaryKeyInt
    {

        public int PrimaryKeyId { get; set; }

    }
}
