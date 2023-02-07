using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface ID_OptionalAggregate
    {

        string PartitionKey { get; set; }

        string OptionalAggregateAttr { get; set; }

        ICollection<ID_MultipleDependent> D_MultipleDependents { get; set; }
    }
}