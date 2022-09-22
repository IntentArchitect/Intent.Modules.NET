using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class G_RequiredCompositeNav : IG_RequiredCompositeNav
    {
        public G_RequiredCompositeNav()
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

        private string _requiredCompNavAttr;

        public string RequiredCompNavAttr
        {
            get { return _requiredCompNavAttr; }
            set
            {
                _requiredCompNavAttr = value;
            }
        }

        private ICollection<G_MultipleDependent> _g_MultipleDependents;

        public virtual ICollection<G_MultipleDependent> G_MultipleDependents
        {
            get
            {
                return _g_MultipleDependents ??= new List<G_MultipleDependent>();
            }
            set
            {
                _g_MultipleDependents = value;
            }
        }


    }
}
