using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class DerivedClassForConcrete : ConcreteBaseClass, IDerivedClassForConcrete, IHasDomainEvent
    {
        public DerivedClassForConcrete()
        {
        }


        private string _derivedAttribute;

        public string DerivedAttribute
        {
            get { return _derivedAttribute; }
            set
            {
                _derivedAttribute = value;
            }
        }

    }
}
