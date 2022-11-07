using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class ConcreteBaseClass : IConcreteBaseClass
    {

        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }

        public string PartitionKey { get; set; }
    }
}
