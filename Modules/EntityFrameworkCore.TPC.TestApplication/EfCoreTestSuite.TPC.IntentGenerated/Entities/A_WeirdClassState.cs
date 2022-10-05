using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class A_WeirdClass : A_OwnedClass, IA_WeirdClass, IHasDomainEvent
    {
        public A_WeirdClass()
        {
        }


        private string _weirdField;

        public string WeirdField
        {
            get { return _weirdField; }
            set
            {
                _weirdField = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
