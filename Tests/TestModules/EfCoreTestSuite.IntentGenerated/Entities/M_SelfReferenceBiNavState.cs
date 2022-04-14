using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
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


        public Guid M_SelfReferenceBiNavId { get; set; }
        private M_SelfReferenceBiNav _m_SelfReferenceBiNav;

        public virtual M_SelfReferenceBiNav M_SelfReferenceBiNav
        {
            get
            {
                return _m_SelfReferenceBiNav;
            }
            set
            {
                _m_SelfReferenceBiNav = value;
            }
        }


    }
}
