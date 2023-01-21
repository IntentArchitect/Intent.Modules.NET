using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class F_OptionalDependent : IF_OptionalDependent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalDependentAttr { get; set; }

        public virtual F_OptionalAggregateNav FOptionalAggregateNav { get; set; }

        IF_OptionalAggregateNav IF_OptionalDependent.FOptionalAggregateNav
        {
            get => FOptionalAggregateNav;
            set => FOptionalAggregateNav = (F_OptionalAggregateNav)value;
        }
    }
}