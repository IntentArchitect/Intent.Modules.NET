using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public interface IA_RequiredComposite
    {
        string RequiredCompAttr { get; set; }

        IA_OptionalDependent A_OptionalDependent { get; set; }

    }
}
