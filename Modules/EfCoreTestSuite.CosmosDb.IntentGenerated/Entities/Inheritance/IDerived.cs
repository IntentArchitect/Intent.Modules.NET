using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public partial interface IDerived : IBase
    {

        string PartitionKey { get; set; }

        string DerivedField1 { get; set; }

        Guid AssociatedId { get; }
        Associated Associated { get; set; }

        ICollection<Composite> Composites { get; set; }

    }
}
