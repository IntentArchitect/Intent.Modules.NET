using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPC.IntentGenerated.Core;
using EfCoreTestSuite.TPC.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_SecondLevel : IPoly_SecondLevel, IHasDomainEvent
    {
        public Poly_SecondLevel()
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

        private string _secondField;

        public string SecondField
        {
            get { return _secondField; }
            set
            {
                _secondField = value;
            }
        }

        private ICollection<Poly_BaseClassNonAbstract> _baseClassNonAbstracts;

        public virtual ICollection<Poly_BaseClassNonAbstract> BaseClassNonAbstracts
        {
            get
            {
                return _baseClassNonAbstracts ??= new List<Poly_BaseClassNonAbstract>();
            }
            set
            {
                _baseClassNonAbstracts = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
