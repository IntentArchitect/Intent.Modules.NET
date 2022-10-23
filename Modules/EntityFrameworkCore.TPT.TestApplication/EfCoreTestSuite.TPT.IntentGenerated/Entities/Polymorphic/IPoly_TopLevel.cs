using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public interface IPoly_TopLevel
    {
        string TopField { get; set; }

        ICollection<IPoly_RootAbstract> RootAbstracts { get; set; }

    }
}
