using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract_Aggr : IPoly_RootAbstract_Aggr
    {

        public Guid Id { get; set; }

        public string AggrField { get; set; }

        public string PartitionKey { get; set; }


    }
}
