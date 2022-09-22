using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class M_SelfReferenceBiNav : IM_SelfReferenceBiNav
    {
        public M_SelfReferenceBiNav()
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

        private string _selfRefBiNavAttr;

        public string SelfRefBiNavAttr
        {
            get { return _selfRefBiNavAttr; }
            set
            {
                _selfRefBiNavAttr = value;
            }
        }


        public Guid? M_SelfReferenceBiNavAssocationId { get; set; }
        private M_SelfReferenceBiNav _m_SelfReferenceBiNavAssocation;

        public virtual M_SelfReferenceBiNav M_SelfReferenceBiNavAssocation
        {
            get
            {
                return _m_SelfReferenceBiNavAssocation;
            }
            set
            {
                _m_SelfReferenceBiNavAssocation = value;
            }
        }

        private ICollection<M_SelfReferenceBiNav> _m_SelfReferenceBiNavs;

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs
        {
            get
            {
                return _m_SelfReferenceBiNavs ??= new List<M_SelfReferenceBiNav>();
            }
            set
            {
                _m_SelfReferenceBiNavs = value;
            }
        }


    }
}
