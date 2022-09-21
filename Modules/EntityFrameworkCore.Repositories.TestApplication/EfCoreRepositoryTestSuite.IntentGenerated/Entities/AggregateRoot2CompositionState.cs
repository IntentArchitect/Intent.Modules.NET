using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot2Composition : IAggregateRoot2Composition
    {
        public AggregateRoot2Composition()
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

        private AggregateRoot2Single _aggregateRoot2Single;

        public virtual AggregateRoot2Single AggregateRoot2Single
        {
            get
            {
                return _aggregateRoot2Single;
            }
            set
            {
                _aggregateRoot2Single = value;
            }
        }

        private AggregateRoot2Nullable _aggregateRoot2Nullable;

        public virtual AggregateRoot2Nullable AggregateRoot2Nullable
        {
            get
            {
                return _aggregateRoot2Nullable;
            }
            set
            {
                _aggregateRoot2Nullable = value;
            }
        }

        private ICollection<AggregateRoot2Collection> _aggregateRoot2Collections;

        public virtual ICollection<AggregateRoot2Collection> AggregateRoot2Collections
        {
            get
            {
                return _aggregateRoot2Collections ??= new List<AggregateRoot2Collection>();
            }
            set
            {
                _aggregateRoot2Collections = value;
            }
        }


    }
}
