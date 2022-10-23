using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class FkBaseClassAssociated : IFkBaseClassAssociated, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkBaseClassCompositeKeyA { get; set; }
        public Guid FkBaseClassCompositeKeyB { get; set; }

        public virtual FkBaseClass FkBaseClass { get; set; }

        IFkBaseClass IFkBaseClassAssociated.FkBaseClass
        {
            get => FkBaseClass;
            set => FkBaseClass = (FkBaseClass)value;
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
