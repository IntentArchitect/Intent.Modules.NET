using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class DerivedClassForAbstractAssociated : IDerivedClassForAbstractAssociated, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }


        public Guid DerivedClassForAbstractId { get; set; }

        public virtual DerivedClassForAbstract DerivedClassForAbstract { get; set; }

        IDerivedClassForAbstract IDerivedClassForAbstractAssociated.DerivedClassForAbstract
        {
            get => DerivedClassForAbstract;
            set => DerivedClassForAbstract = (DerivedClassForAbstract)value;
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
