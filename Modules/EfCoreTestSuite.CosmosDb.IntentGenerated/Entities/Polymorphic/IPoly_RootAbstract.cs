using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial interface IPoly_RootAbstract
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string AbstractField { get; set; }

        string PartitionKey { get; set; }

        Guid? Poly_RootAbstract_AggrId { get; }
        Poly_RootAbstract_Aggr Poly_RootAbstract_Aggr { get; set; }

        Poly_RootAbstract_Comp Poly_RootAbstract_Comp { get; set; }

        Guid? Poly_TopLevelId { get; }
    }
}
