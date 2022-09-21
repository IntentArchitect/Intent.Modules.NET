using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot3AggCollection : IAggregateRoot3AggCollection
    {
        public AggregateRoot3AggCollection()
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


        public Guid AggregateRoot3SingleId { get; set; }
        private AggregateRoot3Single _aggregateRoot3Single;

        public virtual AggregateRoot3Single AggregateRoot3Single
        {
            get
            {
                return _aggregateRoot3Single;
            }
            set
            {
                _aggregateRoot3Single = value;
            }
        }


        public Guid? AggregateRoot3NullableId { get; set; }
        private AggregateRoot3Nullable _aggregateRoot3Nullable;

        public virtual AggregateRoot3Nullable AggregateRoot3Nullable
        {
            get
            {
                return _aggregateRoot3Nullable;
            }
            set
            {
                _aggregateRoot3Nullable = value;
            }
        }

        private ICollection<AggregateRoot3Collection> _aggregateRoot3Collections;

        public virtual ICollection<AggregateRoot3Collection> AggregateRoot3Collections
        {
            get
            {
                return _aggregateRoot3Collections ??= new List<AggregateRoot3Collection>();
            }
            set
            {
                _aggregateRoot3Collections = value;
            }
        }


    }
}
