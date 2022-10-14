using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class DerivedClassForAbstract : AbstractBaseClass, IDerivedClassForAbstract, IHasDomainEvent
    {

        public string DerivedAttribute { get; set; }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
