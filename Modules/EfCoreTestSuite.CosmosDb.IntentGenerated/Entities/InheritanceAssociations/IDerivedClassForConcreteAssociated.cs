using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{

    public interface IDerivedClassForConcreteAssociated
    {
        string AssociatedField { get; set; }

        string PartitionKey { get; set; }

        IDerivedClassForConcrete DerivedClassForConcrete { get; set; }

    }
}
