using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial class ExplicitKeysCompositeForeignKey : IExplicitKeysCompositeForeignKey
    {
        public ExplicitKeysCompositeForeignKey()
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

        private Guid _explicitKeysCompositeKeyCompositeKeyA;

        public Guid ExplicitKeysCompositeKeyCompositeKeyA
        {
            get { return _explicitKeysCompositeKeyCompositeKeyA; }
            set
            {
                _explicitKeysCompositeKeyCompositeKeyA = value;
            }
        }

        private Guid _explicitKeysCompositeKeyCompositeKeyB;

        public Guid ExplicitKeysCompositeKeyCompositeKeyB
        {
            get { return _explicitKeysCompositeKeyCompositeKeyB; }
            set
            {
                _explicitKeysCompositeKeyCompositeKeyB = value;
            }
        }

        private ExplicitKeysCompositeKey _explicitKeysCompositeKey;

        public virtual ExplicitKeysCompositeKey ExplicitKeysCompositeKey
        {
            get
            {
                return _explicitKeysCompositeKey;
            }
            set
            {
                _explicitKeysCompositeKey = value;
            }
        }


    }
}
