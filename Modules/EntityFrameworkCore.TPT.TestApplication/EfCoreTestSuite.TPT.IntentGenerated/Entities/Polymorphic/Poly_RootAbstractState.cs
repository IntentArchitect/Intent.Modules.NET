using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public abstract partial class Poly_RootAbstract : IPoly_RootAbstract, IHasDomainEvent
    {
        public Poly_RootAbstract()
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

        private string _abstractField;

        public string AbstractField
        {
            get { return _abstractField; }
            set
            {
                _abstractField = value;
            }
        }


        public Guid? Poly_RootAbstract_AggrId { get; set; }
        private Poly_RootAbstract_Aggr _poly_RootAbstract_Aggr;

        public virtual Poly_RootAbstract_Aggr Poly_RootAbstract_Aggr
        {
            get
            {
                return _poly_RootAbstract_Aggr;
            }
            set
            {
                _poly_RootAbstract_Aggr = value;
            }
        }

        private Poly_RootAbstract_Comp _poly_RootAbstract_Comp;

        public virtual Poly_RootAbstract_Comp Poly_RootAbstract_Comp
        {
            get
            {
                return _poly_RootAbstract_Comp;
            }
            set
            {
                _poly_RootAbstract_Comp = value;
            }
        }


        public Guid? TopLevelId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
