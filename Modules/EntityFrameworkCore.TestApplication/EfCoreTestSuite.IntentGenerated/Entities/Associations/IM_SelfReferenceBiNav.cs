using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IM_SelfReferenceBiNav
    {

        string SelfRefBiNavAttr { get; set; }

        IM_SelfReferenceBiNav MSelfReferenceBiNavDst { get; set; }

        ICollection<IM_SelfReferenceBiNav> MSelfReferenceBiNavs { get; set; }

    }
}
