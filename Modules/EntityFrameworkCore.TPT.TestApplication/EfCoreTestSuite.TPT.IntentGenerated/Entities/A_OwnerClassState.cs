using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities
{

    public partial class A_OwnerClass : IA_OwnerClass, IHasDomainEvent
    {
        public A_OwnerClass()
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

        private ICollection<A_OwnedClass> _a_OwnedClasses;

        public virtual ICollection<A_OwnedClass> A_OwnedClasses
        {
            get
            {
                return _a_OwnedClasses ??= new List<A_OwnedClass>();
            }
            set
            {
                _a_OwnedClasses = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();


    }
}
