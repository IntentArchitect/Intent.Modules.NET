using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class C_MultipleDependent : IC_MultipleDependent
    {

        public Guid Id { get; set; }

        public string MultipleDependentAttr { get; set; }
    }
}
