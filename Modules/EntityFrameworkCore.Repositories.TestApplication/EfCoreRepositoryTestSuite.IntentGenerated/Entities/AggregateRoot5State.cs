using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot5 : IAggregateRoot5
    {
        public AggregateRoot5()
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

        private AggregateRoot5EntityWithRepo _aggregateRoot5EntityWithRepo;

        public virtual AggregateRoot5EntityWithRepo AggregateRoot5EntityWithRepo
        {
            get
            {
                return _aggregateRoot5EntityWithRepo;
            }
            set
            {
                _aggregateRoot5EntityWithRepo = value;
            }
        }


    }
}
