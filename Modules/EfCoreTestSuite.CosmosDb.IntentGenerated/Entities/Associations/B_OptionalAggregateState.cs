using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class B_OptionalAggregate : IB_OptionalAggregate
    {

        public Guid Id { get; set; }

        public string OptionalAggregateAttr { get; set; }

        public string PartitionKey { get; set; }

        public virtual B_OptionalDependent B_OptionalDependent { get; set; }

        IB_OptionalDependent IB_OptionalAggregate.B_OptionalDependent
        {
            get => B_OptionalDependent;
            set => B_OptionalDependent = (B_OptionalDependent)value;
        }
        public Guid? B_OptionalDependentId { get; set; }

    }
}
