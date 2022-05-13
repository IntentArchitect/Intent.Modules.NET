using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
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


        public Guid? M_SelfReferenceBiNavDstId { get; set; }
        private M_SelfReferenceBiNav _m_SelfReferenceBiNavDst;

        public virtual M_SelfReferenceBiNav M_SelfReferenceBiNavDst
        {
            get
            {
                return _m_SelfReferenceBiNavDst;
            }
            set
            {
                _m_SelfReferenceBiNavDst = value;
            }
        }

        private ICollection<M_SelfReferenceBiNav> _m_SelfReferenceBiNavs;

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs
        {
            get
            {
                return _m_SelfReferenceBiNavs ?? (_m_SelfReferenceBiNavs = new List<M_SelfReferenceBiNav>());
            }
            set
            {
                _m_SelfReferenceBiNavs = value;
            }
        }


    }
}
