using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_TopLevel : IPoly_TopLevel, IHasDomainEvent
    {
        public Poly_TopLevel()
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

        private string _topField;

        public string TopField
        {
            get { return _topField; }
            set
            {
                _topField = value;
            }
        }

        private ICollection<Poly_RootAbstract> _rootAbstracts;

        public virtual ICollection<Poly_RootAbstract> RootAbstracts
        {
            get
            {
                return _rootAbstracts ??= new List<Poly_RootAbstract>();
            }
            set
            {
                _rootAbstracts = value;
            }
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
