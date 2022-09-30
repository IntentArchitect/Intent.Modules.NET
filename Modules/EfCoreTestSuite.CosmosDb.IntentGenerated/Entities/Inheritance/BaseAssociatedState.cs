using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public partial class BaseAssociated : IBaseAssociated
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string BaseAssociatedField1 { get; set; }


    }
}
