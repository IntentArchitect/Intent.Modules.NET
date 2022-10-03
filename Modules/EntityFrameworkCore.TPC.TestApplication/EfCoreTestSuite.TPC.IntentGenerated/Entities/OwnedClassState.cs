using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class OwnedClass : AbstractClass, IOwnedClass
    {
        public OwnedClass()
        {
        }


        private string _ownedField;

        public string OwnedField
        {
            get { return _ownedField; }
            set
            {
                _ownedField = value;
            }
        }


        public Guid OwnerClassId { get; set; }
    }
}
