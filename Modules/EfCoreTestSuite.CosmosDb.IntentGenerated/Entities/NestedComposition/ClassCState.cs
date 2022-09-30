using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition
{

    public partial class ClassC : IClassC
    {

        public Guid Id { get; set; }

        public string ClassCAttr { get; set; }


    }
}
