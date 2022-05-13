using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities
{

    public partial class PK_ExplicitKeys_CompositeKey : IPK_ExplicitKeys_CompositeKey
    {
        public PK_ExplicitKeys_CompositeKey()
        {
        }


        private Guid _compositeKeyA;

        public Guid CompositeKeyA
        {
            get { return _compositeKeyA; }
            set
            {
                _compositeKeyA = value;
            }
        }

        private Guid _compositeKeyB;

        public Guid CompositeKeyB
        {
            get { return _compositeKeyB; }
            set
            {
                _compositeKeyB = value;
            }
        }


    }
}
