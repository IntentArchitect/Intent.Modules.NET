using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class A_RequiredComposite : IA_RequiredComposite
    {
        public A_RequiredComposite()
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

        private string _requiredCompositeAttr;

        public string RequiredCompositeAttr
        {
            get { return _requiredCompositeAttr; }
            set
            {
                _requiredCompositeAttr = value;
            }
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

        private A_OptionalDependent _a_OptionalDependent;

        public virtual A_OptionalDependent A_OptionalDependent
        {
            get
            {
                return _a_OptionalDependent;
            }
            set
            {
                _a_OptionalDependent = value;
            }
        }


    }
}
