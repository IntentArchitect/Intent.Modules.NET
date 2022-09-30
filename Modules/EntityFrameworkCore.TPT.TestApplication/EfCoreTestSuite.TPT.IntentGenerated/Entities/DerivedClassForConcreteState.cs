using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities
{

    public partial class DerivedClassForConcrete : ConcreteBaseClass, IDerivedClassForConcrete
    {

        public string DerivedAttribute { get; set; }

    }
}
