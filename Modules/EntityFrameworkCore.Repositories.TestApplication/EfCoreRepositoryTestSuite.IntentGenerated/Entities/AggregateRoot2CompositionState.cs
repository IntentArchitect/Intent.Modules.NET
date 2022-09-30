using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot2Composition : IAggregateRoot2Composition
    {

        public Guid Id { get; set; }

        public virtual AggregateRoot2Single AggregateRoot2Single { get; set; }

        IAggregateRoot2Single IAggregateRoot2Composition.AggregateRoot2Single
        {
            get => AggregateRoot2Single;
            set => AggregateRoot2Single = (AggregateRoot2Single)value;
        }

        public virtual AggregateRoot2Nullable AggregateRoot2Nullable { get; set; }

        IAggregateRoot2Nullable IAggregateRoot2Composition.AggregateRoot2Nullable
        {
            get => AggregateRoot2Nullable;
            set => AggregateRoot2Nullable = (AggregateRoot2Nullable)value;
        }

        public virtual ICollection<AggregateRoot2Collection> AggregateRoot2Collections { get; set; } = new List<AggregateRoot2Collection>();

        ICollection<IAggregateRoot2Collection> IAggregateRoot2Composition.AggregateRoot2Collections
        {
            get => AggregateRoot2Collections.CreateWrapper<IAggregateRoot2Collection, AggregateRoot2Collection>();
            set => AggregateRoot2Collections = value.Cast<AggregateRoot2Collection>().ToList();
        }


    }
}
