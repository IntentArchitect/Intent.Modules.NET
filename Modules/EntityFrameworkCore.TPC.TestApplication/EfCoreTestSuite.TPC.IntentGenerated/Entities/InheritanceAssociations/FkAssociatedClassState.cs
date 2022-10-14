using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class FkAssociatedClass : IFkAssociatedClass, IHasDomainEvent
    {

        public Guid Id { get; set; }

        private string _associatedField;

        public string AssociatedField
        {
            get { return _associatedField; }
            set
            {
                _associatedField = value;
            }
        }

        public Guid FkDerivedClassCompositeKeyA { get; set; }
        public Guid FkDerivedClassCompositeKeyB { get; set; }

        public virtual FkDerivedClass FkDerivedClass { get; set; }

        IFkDerivedClass IFkAssociatedClass.FkDerivedClass
        {
            get => FkDerivedClass;
            set => FkDerivedClass = (FkDerivedClass)value;
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
