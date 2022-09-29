using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class E2_RequiredDependent : IE2_RequiredDependent
    {
        public E2_RequiredDependent()
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

        private string _reqDepAttr;

        public string ReqDepAttr
        {
            get { return _reqDepAttr; }
            set
            {
                _reqDepAttr = value;
            }
        }

        private E2_RequiredCompositeNav _e2_RequiredCompositeNav;

        public virtual E2_RequiredCompositeNav E2_RequiredCompositeNav
        {
            get
            {
                return _e2_RequiredCompositeNav;
            }
            set
            {
                _e2_RequiredCompositeNav = value;
            }
        }


    }
}
