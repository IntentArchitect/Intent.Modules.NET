using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public interface IDerived : IBase
    {
        string PartitionKey { get; set; }

        string DerivedField1 { get; set; }

        IAssociated Associated { get; set; }

        ICollection<IComposite> Composites { get; set; }

    }
}
