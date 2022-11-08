using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.InheritanceAssociations
{

    public interface IDerivedClassForConcreteAssociated : IHasDomainEvent
    {
        string AssociatedField { get; set; }

        IDerivedClassForConcrete DerivedClassForConcrete { get; set; }

    }
}
