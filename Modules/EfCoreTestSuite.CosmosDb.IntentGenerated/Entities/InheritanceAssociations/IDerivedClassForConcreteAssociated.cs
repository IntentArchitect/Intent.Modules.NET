using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{

    public partial interface IDerivedClassForConcreteAssociated
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string AssociatedField { get; set; }

        string PartitionKey { get; set; }

        Guid DerivedClassForConcreteId { get; }
        DerivedClassForConcrete DerivedClassForConcrete { get; set; }

    }
}
