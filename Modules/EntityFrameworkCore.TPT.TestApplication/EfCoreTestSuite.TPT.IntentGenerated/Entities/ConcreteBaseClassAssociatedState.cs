using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities
{

    public partial class ConcreteBaseClassAssociated : IConcreteBaseClassAssociated, IHasDomainEvent
    {
        public ConcreteBaseClassAssociated()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private string _associatedField;

        public string AssociatedField
        {
            get { return _associatedField; }
            set
            {
                _associatedField = value;
            }
        }


        public Guid ConcreteBaseClassId { get; set; }
        private ConcreteBaseClass _concreteBaseClass;

        public virtual ConcreteBaseClass ConcreteBaseClass
        {
            get
            {
                return _concreteBaseClass;
            }
            set
            {
                _concreteBaseClass = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
