using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot4AggNullable : IAggregateRoot4AggNullable
    {

        public Guid Id { get; set; }

        public virtual AggregateRoot4Single AggregateRoot4Single { get; set; }

        IAggregateRoot4Single IAggregateRoot4AggNullable.AggregateRoot4Single
        {
            get => AggregateRoot4Single;
            set => AggregateRoot4Single = (AggregateRoot4Single)value;
        }

        public virtual ICollection<AggregateRoot4Collection> AggregateRoot4Collections { get; set; } = new List<AggregateRoot4Collection>();

        ICollection<IAggregateRoot4Collection> IAggregateRoot4AggNullable.AggregateRoot4Collections
        {
            get => AggregateRoot4Collections.CreateWrapper<IAggregateRoot4Collection, AggregateRoot4Collection>();
            set => AggregateRoot4Collections = value.Cast<AggregateRoot4Collection>().ToList();
        }

        public virtual AggregateRoot4Nullable AggregateRoot4Nullable { get; set; }

        IAggregateRoot4Nullable IAggregateRoot4AggNullable.AggregateRoot4Nullable
        {
            get => AggregateRoot4Nullable;
            set => AggregateRoot4Nullable = (AggregateRoot4Nullable)value;
        }
        public Guid? AggregateRoot4NullableId { get; set; }

    }
}
