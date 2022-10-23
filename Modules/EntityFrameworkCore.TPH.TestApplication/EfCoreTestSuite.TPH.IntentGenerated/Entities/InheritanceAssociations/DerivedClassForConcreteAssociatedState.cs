using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class DerivedClassForConcreteAssociated : IDerivedClassForConcreteAssociated, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }


        public Guid DerivedClassForConcreteId { get; set; }

        public virtual DerivedClassForConcrete DerivedClassForConcrete { get; set; }

        IDerivedClassForConcrete IDerivedClassForConcreteAssociated.DerivedClassForConcrete
        {
            get => DerivedClassForConcrete;
            set => DerivedClassForConcrete = (DerivedClassForConcrete)value;
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
