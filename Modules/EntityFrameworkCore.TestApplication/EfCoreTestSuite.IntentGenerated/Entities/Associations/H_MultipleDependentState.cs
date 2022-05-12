using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class H_MultipleDependent : IH_MultipleDependent
    {
        public H_MultipleDependent()
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


        public Guid? H_OptionalAggregateNavId { get; set; }
        private H_OptionalAggregateNav _h_OptionalAggregateNav;

        public virtual H_OptionalAggregateNav H_OptionalAggregateNav
        {
            get
            {
                return _h_OptionalAggregateNav;
            }
            set
            {
                _h_OptionalAggregateNav = value;
            }
        }


    }
}
