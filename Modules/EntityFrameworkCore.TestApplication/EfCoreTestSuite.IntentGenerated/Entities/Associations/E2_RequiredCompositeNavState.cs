using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class E2_RequiredCompositeNav : IE2_RequiredCompositeNav
    {
        public E2_RequiredCompositeNav()
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

        private string _reqCompNavAttr;

        public string ReqCompNavAttr
        {
            get { return _reqCompNavAttr; }
            set
            {
                _reqCompNavAttr = value;
            }
        }

        private E2_RequiredDependent _e2_RequiredDependent;

        public virtual E2_RequiredDependent E2_RequiredDependent
        {
            get
            {
                return _e2_RequiredDependent;
            }
            set
            {
                _e2_RequiredDependent = value;
            }
        }


    }
}
