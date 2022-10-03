using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class OwnerClass : IOwnerClass, IHasDomainEvent
    {
        public OwnerClass()
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

        private string _ownerField;

        public string OwnerField
        {
            get { return _ownerField; }
            set
            {
                _ownerField = value;
            }
        }

        private ICollection<OwnedClass> _ownedClasses;

        public virtual ICollection<OwnedClass> OwnedClasses
        {
            get
            {
                return _ownedClasses ??= new List<OwnedClass>();
            }
            set
            {
                _ownedClasses = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
