using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class H_OptionalAggregateNav : IH_OptionalAggregateNav
    {
        public H_OptionalAggregateNav()
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

        private string _partitionKey;

        public string PartitionKey
        {
            get { return _partitionKey; }
            set
            {
                _partitionKey = value;
            }
        }

        private string _optionalAggrNavAttr;

        public string OptionalAggrNavAttr
        {
            get { return _optionalAggrNavAttr; }
            set
            {
                _optionalAggrNavAttr = value;
            }
        }

        private ICollection<H_MultipleDependent> _h_MultipleDependents;

        public virtual ICollection<H_MultipleDependent> H_MultipleDependents
        {
            get
            {
                return _h_MultipleDependents ??= new List<H_MultipleDependent>();
            }
            set
            {
                _h_MultipleDependents = value;
            }
        }


    }
}
