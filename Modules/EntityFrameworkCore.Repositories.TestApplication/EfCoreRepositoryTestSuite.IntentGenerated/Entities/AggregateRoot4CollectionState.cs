using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot4Collection : IAggregateRoot4Collection
    {

        public Guid Id { get; set; }


        public Guid? AggregateRoot4AggNullableId { get; set; }
    }
}
