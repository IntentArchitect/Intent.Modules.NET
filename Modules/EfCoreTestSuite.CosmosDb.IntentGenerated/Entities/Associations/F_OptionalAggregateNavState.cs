using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class F_OptionalAggregateNav : IF_OptionalAggregateNav
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual F_OptionalDependent F_OptionalDependent { get; set; }

        IF_OptionalDependent IF_OptionalAggregateNav.F_OptionalDependent
        {
            get => F_OptionalDependent;
            set => F_OptionalDependent = (F_OptionalDependent)value;
        }
        public Guid? F_OptionalDependentId { get; set; }

    }
}
