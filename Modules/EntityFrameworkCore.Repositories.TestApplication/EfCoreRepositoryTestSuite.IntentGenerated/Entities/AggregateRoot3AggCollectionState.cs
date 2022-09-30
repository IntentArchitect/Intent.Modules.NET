using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot3AggCollection : IAggregateRoot3AggCollection
    {

        public Guid Id { get; set; }


        public Guid AggregateRoot3SingleId { get; set; }

        public virtual AggregateRoot3Single AggregateRoot3Single { get; set; }

        IAggregateRoot3Single IAggregateRoot3AggCollection.AggregateRoot3Single
        {
            get => AggregateRoot3Single;
            set => AggregateRoot3Single = (AggregateRoot3Single)value;
        }


        public Guid? AggregateRoot3NullableId { get; set; }

        public virtual AggregateRoot3Nullable AggregateRoot3Nullable { get; set; }

        IAggregateRoot3Nullable IAggregateRoot3AggCollection.AggregateRoot3Nullable
        {
            get => AggregateRoot3Nullable;
            set => AggregateRoot3Nullable = (AggregateRoot3Nullable)value;
        }

        public virtual ICollection<AggregateRoot3Collection> AggregateRoot3Collections { get; set; } = new List<AggregateRoot3Collection>();

        ICollection<IAggregateRoot3Collection> IAggregateRoot3AggCollection.AggregateRoot3Collections
        {
            get => AggregateRoot3Collections.CreateWrapper<IAggregateRoot3Collection, AggregateRoot3Collection>();
            set => AggregateRoot3Collections = value.Cast<AggregateRoot3Collection>().ToList();
        }


    }
}
