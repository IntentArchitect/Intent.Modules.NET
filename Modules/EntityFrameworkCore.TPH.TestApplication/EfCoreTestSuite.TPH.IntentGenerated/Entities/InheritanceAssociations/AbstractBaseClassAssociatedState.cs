using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class AbstractBaseClassAssociated : IAbstractBaseClassAssociated, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }


        public Guid AbstractBaseClassId { get; set; }

        public virtual AbstractBaseClass AbstractBaseClass { get; set; }

        IAbstractBaseClass IAbstractBaseClassAssociated.AbstractBaseClass
        {
            get => AbstractBaseClass;
            set => AbstractBaseClass = (AbstractBaseClass)value;
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
