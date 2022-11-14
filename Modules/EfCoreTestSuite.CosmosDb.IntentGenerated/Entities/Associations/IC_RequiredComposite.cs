using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IC_RequiredComposite
    {

        string PartitionKey { get; set; }

        string RequiredCompositeAttr { get; set; }

        ICollection<IC_MultipleDependent> C_MultipleDependents { get; set; }
    }
}