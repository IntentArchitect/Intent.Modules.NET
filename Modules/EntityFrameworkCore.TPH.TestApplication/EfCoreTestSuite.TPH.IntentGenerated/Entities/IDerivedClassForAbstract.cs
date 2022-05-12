using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities
{

    public partial interface IDerivedClassForAbstract : IAbstractBaseClass
    {

        string DerivedAttribute { get; set; }

    }
}
