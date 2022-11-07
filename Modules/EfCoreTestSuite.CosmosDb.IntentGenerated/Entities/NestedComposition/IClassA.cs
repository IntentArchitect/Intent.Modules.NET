using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{
    public interface IClassA
    {
        string PartitionKey { get; set; }

        string ClassAAttr { get; set; }

        ICollection<IClassB> ClassBS { get; set; }
    }
}