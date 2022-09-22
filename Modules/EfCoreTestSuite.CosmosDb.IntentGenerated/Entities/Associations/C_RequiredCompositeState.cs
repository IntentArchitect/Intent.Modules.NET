using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class C_RequiredComposite : IC_RequiredComposite
    {
        public C_RequiredComposite()
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

        private string _requiredCompositeAttr;

        public string RequiredCompositeAttr
        {
            get { return _requiredCompositeAttr; }
            set
            {
                _requiredCompositeAttr = value;
            }
        }

        private ICollection<C_MultipleDependent> _c_MultipleDependents;

        public virtual ICollection<C_MultipleDependent> C_MultipleDependents
        {
            get
            {
                return _c_MultipleDependents ??= new List<C_MultipleDependent>();
            }
            set
            {
                _c_MultipleDependents = value;
            }
        }


    }
}
