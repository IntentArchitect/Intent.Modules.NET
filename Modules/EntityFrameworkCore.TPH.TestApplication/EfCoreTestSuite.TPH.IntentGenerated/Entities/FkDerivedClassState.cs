using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities
{

    public partial class FkDerivedClass : FkBaseClass, IFkDerivedClass
    {
        public FkDerivedClass()
        {
        }



    }
}
