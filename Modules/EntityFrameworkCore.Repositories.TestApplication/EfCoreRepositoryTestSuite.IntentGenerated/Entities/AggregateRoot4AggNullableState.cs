using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot4AggNullable : IAggregateRoot4AggNullable
    {
        public AggregateRoot4AggNullable()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private AggregateRoot4Single _aggregateRoot4Single;

        public virtual AggregateRoot4Single AggregateRoot4Single
        {
            get
            {
                return _aggregateRoot4Single;
            }
            set
            {
                _aggregateRoot4Single = value;
            }
        }

        private ICollection<AggregateRoot4Collection> _aggregateRoot4Collections;

        public virtual ICollection<AggregateRoot4Collection> AggregateRoot4Collections
        {
            get
            {
                return _aggregateRoot4Collections ??= new List<AggregateRoot4Collection>();
            }
            set
            {
                _aggregateRoot4Collections = value;
            }
        }

        private AggregateRoot4Nullable _aggregateRoot4Nullable;

        public virtual AggregateRoot4Nullable AggregateRoot4Nullable
        {
            get
            {
                return _aggregateRoot4Nullable;
            }
            set
            {
                _aggregateRoot4Nullable = value;
            }
        }
        public Guid? AggregateRoot4NullableId { get; set; }

    }
}
