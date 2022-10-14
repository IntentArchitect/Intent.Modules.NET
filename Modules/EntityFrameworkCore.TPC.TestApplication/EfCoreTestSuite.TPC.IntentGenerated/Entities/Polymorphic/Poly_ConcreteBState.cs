using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_ConcreteB : Poly_BaseClassNonAbstract, IPoly_ConcreteB, IHasDomainEvent
    {
        public Poly_ConcreteB()
        {
        }


        private string _concreteField;

        public string ConcreteField
        {
            get { return _concreteField; }
            set
            {
                _concreteField = value;
            }
        }

    }
}
