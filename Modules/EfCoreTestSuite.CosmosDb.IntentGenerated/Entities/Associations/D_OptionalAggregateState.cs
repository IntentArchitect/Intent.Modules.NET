using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class D_OptionalAggregate : ID_OptionalAggregate
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggregateAttr { get; set; }

        public virtual ICollection<D_MultipleDependent> D_MultipleDependents { get; set; } = new List<D_MultipleDependent>();

        ICollection<ID_MultipleDependent> ID_OptionalAggregate.D_MultipleDependents
        {
            get => D_MultipleDependents.CreateWrapper<ID_MultipleDependent, D_MultipleDependent>();
            set => D_MultipleDependents = value.Cast<D_MultipleDependent>().ToList();
        }


    }
}
