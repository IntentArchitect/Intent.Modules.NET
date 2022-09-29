using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{

    public partial interface IClassB
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string ClassBAttr { get; set; }

        ClassC ClassC { get; set; }

        ICollection<ClassD> ClassDS { get; set; }

        Guid ClassAId { get; }
    }
}
