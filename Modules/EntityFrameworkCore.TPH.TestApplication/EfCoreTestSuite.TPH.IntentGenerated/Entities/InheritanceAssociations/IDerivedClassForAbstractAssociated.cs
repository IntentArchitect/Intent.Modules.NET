using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations
{

    public interface IDerivedClassForAbstractAssociated : IHasDomainEvent
    {

        string AssociatedField { get; set; }

        IDerivedClassForAbstract DerivedClassForAbstract { get; set; }

    }
}
