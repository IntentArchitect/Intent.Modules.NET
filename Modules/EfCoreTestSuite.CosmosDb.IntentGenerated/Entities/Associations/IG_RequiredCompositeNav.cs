using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IG_RequiredCompositeNav
    {

        string PartitionKey { get; set; }

        string RequiredCompNavAttr { get; set; }

        ICollection<IG_MultipleDependent> G_MultipleDependents { get; set; }
    }
}