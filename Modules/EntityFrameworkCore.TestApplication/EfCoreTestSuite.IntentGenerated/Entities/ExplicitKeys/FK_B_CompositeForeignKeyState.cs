using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class FK_B_CompositeForeignKey : IFK_B_CompositeForeignKey
    {
        public FK_B_CompositeForeignKey()
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

        private Guid _pK_CompositeKeyCompositeKeyA;

        public Guid PK_CompositeKeyCompositeKeyA
        {
            get { return _pK_CompositeKeyCompositeKeyA; }
            set
            {
                _pK_CompositeKeyCompositeKeyA = value;
            }
        }

        private Guid _pK_CompositeKeyCompositeKeyB;

        public Guid PK_CompositeKeyCompositeKeyB
        {
            get { return _pK_CompositeKeyCompositeKeyB; }
            set
            {
                _pK_CompositeKeyCompositeKeyB = value;
            }
        }

        private PK_B_CompositeKey _pK_CompositeKey;

        public virtual PK_B_CompositeKey PK_CompositeKey
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
