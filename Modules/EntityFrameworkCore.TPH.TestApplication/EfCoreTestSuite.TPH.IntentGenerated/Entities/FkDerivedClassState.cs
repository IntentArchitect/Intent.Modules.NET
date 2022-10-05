using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities
{

    public partial class FkDerivedClass : FkBaseClass, IFkDerivedClass, IHasDomainEvent
    {
        public FkDerivedClass()
        {
        }


        private string _derivedField;

        public string DerivedField
        {
            get { return _derivedField; }
            set
            {
                _derivedField = value;
            }
        }



    }
}
