using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_BaseClassNonAbstract : Poly_RootAbstract, IPoly_BaseClassNonAbstract, IHasDomainEvent
    {
        public Poly_BaseClassNonAbstract()
        {
        }


        private string _baseField;

        public string BaseField
        {
            get { return _baseField; }
            set
            {
                _baseField = value;
            }
        }


        public Guid? SecondLevelId { get; set; }
    }
}
