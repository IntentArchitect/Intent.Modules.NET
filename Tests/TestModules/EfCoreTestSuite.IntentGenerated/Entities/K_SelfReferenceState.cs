using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class K_SelfReference : IK_SelfReference
    {
        public K_SelfReference()
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


        public Guid? K_SelfReferenceId { get; set; }
        private K_SelfReference _k_SelfReference;

        public virtual K_SelfReference K_SelfReference
        {
            get
            {
                return _k_SelfReference;
            }
            set
            {
                _k_SelfReference = value;
            }
        }


    }
}
