using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class F_OptionalDependent : IF_OptionalDependent
    {
        public F_OptionalDependent()
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

        private F_OptionalAggregateNav _f_OptionalAggregateNav;

        public virtual F_OptionalAggregateNav F_OptionalAggregateNav
        {
            get
            {
                return _f_OptionalAggregateNav;
            }
            set
            {
                _f_OptionalAggregateNav = value;
            }
        }


    }
}
