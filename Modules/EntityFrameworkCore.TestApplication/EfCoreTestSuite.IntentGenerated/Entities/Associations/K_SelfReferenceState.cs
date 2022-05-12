using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
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


        public Guid? K_SelfReferenceAssociationId { get; set; }
        private K_SelfReference _k_SelfReferenceAssociation;

        public virtual K_SelfReference K_SelfReferenceAssociation
        {
            get
            {
                return _k_SelfReferenceAssociation;
            }
            set
            {
                _k_SelfReferenceAssociation = value;
            }
        }


    }
}
