using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{
    public interface IClassB
    {

        string ClassBAttr { get; set; }

        IClassC ClassC { get; set; }

        ICollection<IClassD> ClassDS { get; set; }
    }
}