using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities
{

    public partial class FkBaseClassAssociated : IFkBaseClassAssociated, IHasDomainEvent
    {
        public FkBaseClassAssociated()
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

        public Guid FkBaseClassCompositeKeyA { get; set; }
        public Guid FkBaseClassCompositeKeyB { get; set; }
        private FkBaseClass _fkBaseClass;

        public virtual FkBaseClass FkBaseClass
        {
            get
            {
                return _fkBaseClass;
            }
            set
            {
                _fkBaseClass = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
