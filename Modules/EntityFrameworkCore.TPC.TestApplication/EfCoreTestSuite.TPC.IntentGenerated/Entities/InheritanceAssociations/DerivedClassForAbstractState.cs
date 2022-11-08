using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class DerivedClassForAbstract : AbstractBaseClass, IDerivedClassForAbstract
    {
        public Guid Id { get; set; }

        public string DerivedAttribute { get; set; }
    }
}
