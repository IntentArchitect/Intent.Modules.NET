using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class D_MultipleDependent : ID_MultipleDependent
    {

        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? DOptionalAggregateId { get; set; }
    }
}
