using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class L_SelfReferenceMultiple : IL_SelfReferenceMultiple
    {
        public L_SelfReferenceMultiple()
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

        private ICollection<L_SelfReferenceMultiple> _l_SelfReferenceMultiplesDst;

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst
        {
            get
            {
                return _l_SelfReferenceMultiplesDst ??= new List<L_SelfReferenceMultiple>();
            }
            set
            {
                _l_SelfReferenceMultiplesDst = value;
            }
        }

        protected virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesSrc { get; set; }


    }
}
