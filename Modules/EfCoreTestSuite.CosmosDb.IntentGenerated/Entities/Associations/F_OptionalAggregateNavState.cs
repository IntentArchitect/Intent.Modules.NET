using System;
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

        public Guid? FOptionalDependentId { get; set; }

        public virtual F_OptionalDependent FOptionalDependent { get; set; }

        IF_OptionalDependent IF_OptionalAggregateNav.FOptionalDependent
        {
            get => FOptionalDependent;
            set => FOptionalDependent = (F_OptionalDependent)value;
        }
    }
}