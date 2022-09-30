using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot2Collection : IAggregateRoot2Collection
    {

        public Guid Id { get; set; }


        public Guid AggregateRoot2CompositionId { get; set; }
    }
}
