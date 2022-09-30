using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot3Collection : IAggregateRoot3Collection
    {

        public Guid Id { get; set; }

        private ICollection<AggregateRoot3AggCollection> AggregateRoot3AggCollections { get; set; }

    }
}
