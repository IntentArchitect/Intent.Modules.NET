using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys
{

    public partial class PK_PrimaryKeyLong : IPK_PrimaryKeyLong
    {
        public PK_PrimaryKeyLong()
        {
        }


        private long _primaryKeyLong;

        public long PrimaryKeyLong
        {
            get { return _primaryKeyLong; }
            set
            {
                _primaryKeyLong = value;
            }
        }

    }
}
