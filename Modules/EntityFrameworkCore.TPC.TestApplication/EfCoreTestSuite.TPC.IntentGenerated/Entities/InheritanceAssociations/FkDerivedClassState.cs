using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class FkDerivedClass : FkBaseClass, IFkDerivedClass
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
