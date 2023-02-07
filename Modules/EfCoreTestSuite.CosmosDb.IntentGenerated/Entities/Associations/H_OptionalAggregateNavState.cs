using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class H_OptionalAggregateNav : IH_OptionalAggregateNav
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual ICollection<H_MultipleDependent> H_MultipleDependents { get; set; } = new List<H_MultipleDependent>();

        ICollection<IH_MultipleDependent> IH_OptionalAggregateNav.H_MultipleDependents
        {
            get => H_MultipleDependents.CreateWrapper<IH_MultipleDependent, H_MultipleDependent>();
            set => H_MultipleDependents = value.Cast<H_MultipleDependent>().ToList();
        }
    }
}