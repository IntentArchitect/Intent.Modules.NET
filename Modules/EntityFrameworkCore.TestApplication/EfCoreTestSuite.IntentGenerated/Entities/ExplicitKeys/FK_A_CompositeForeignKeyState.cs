using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class FK_A_CompositeForeignKey : IFK_A_CompositeForeignKey
    {
        public FK_A_CompositeForeignKey()
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

        private Guid _foreignCompositeKeyA;

        public Guid ForeignCompositeKeyA
        {
            get { return _foreignCompositeKeyA; }
            set
            {
                _foreignCompositeKeyA = value;
            }
        }

        private Guid _foreignCompositeKeyB;

        public Guid ForeignCompositeKeyB
        {
            get { return _foreignCompositeKeyB; }
            set
            {
                _foreignCompositeKeyB = value;
            }
        }


        private PK_A_CompositeKey _pK_CompositeKey;

        public virtual PK_A_CompositeKey PK_CompositeKey
        {
            get
            {
                return _pK_CompositeKey;
            }
            set
            {
                _pK_CompositeKey = value;
            }
        }


    }
}
