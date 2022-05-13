using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial class FK_ExplicitKeys_CompositeForeignKey : IFK_ExplicitKeys_CompositeForeignKey
    {
        public FK_ExplicitKeys_CompositeForeignKey()
        {
        }


        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        private Guid _pkExplicitkeysCompositekeyCompositeKeyA;

        public Guid PkExplicitkeysCompositekeyCompositeKeyA
        {
            get { return _pkExplicitkeysCompositekeyCompositeKeyA; }
            set
            {
                _pkExplicitkeysCompositekeyCompositeKeyA = value;
            }
        }

        private Guid _pkExplicitkeysCompositekeyCompositeKeyB;

        public Guid PkExplicitkeysCompositekeyCompositeKeyB
        {
            get { return _pkExplicitkeysCompositekeyCompositeKeyB; }
            set
            {
                _pkExplicitkeysCompositekeyCompositeKeyB = value;
            }
        }

        public Guid PK_ExplicitKeys_CompositeKeyCompositeKeyA { get; set; }
        public Guid PK_ExplicitKeys_CompositeKeyCompositeKeyB { get; set; }
        private PK_ExplicitKeys_CompositeKey _pK_ExplicitKeys_CompositeKey;

        public virtual PK_ExplicitKeys_CompositeKey PK_ExplicitKeys_CompositeKey
        {
            get
            {
                return _pK_ExplicitKeys_CompositeKey;
            }
            set
            {
                _pK_ExplicitKeys_CompositeKey = value;
            }
        }


    }
}
