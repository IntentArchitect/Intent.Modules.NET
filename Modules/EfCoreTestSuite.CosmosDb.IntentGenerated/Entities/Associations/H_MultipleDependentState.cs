using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class H_MultipleDependent : IH_MultipleDependent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? H_OptionalAggregateNavId { get; set; }

        public virtual H_OptionalAggregateNav H_OptionalAggregateNav { get; set; }

        IH_OptionalAggregateNav IH_MultipleDependent.H_OptionalAggregateNav
        {
            get => H_OptionalAggregateNav;
            set => H_OptionalAggregateNav = (H_OptionalAggregateNav)value;
        }
    }
}