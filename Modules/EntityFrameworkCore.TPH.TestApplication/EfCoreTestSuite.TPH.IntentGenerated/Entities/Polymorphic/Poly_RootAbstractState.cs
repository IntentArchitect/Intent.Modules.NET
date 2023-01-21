using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract : IPoly_RootAbstract, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AbstractField { get; set; }

        public Guid? PolyRootAbstractAggrId { get; set; }

        public virtual Poly_RootAbstract_Aggr PolyRootAbstractAggr { get; set; }

        IPoly_RootAbstract_Aggr IPoly_RootAbstract.PolyRootAbstractAggr
        {
            get => PolyRootAbstractAggr;
            set => PolyRootAbstractAggr = (Poly_RootAbstract_Aggr)value;
        }

        public virtual Poly_RootAbstract_Comp PolyRootAbstractComp { get; set; }

        IPoly_RootAbstract_Comp IPoly_RootAbstract.PolyRootAbstractComp
        {
            get => PolyRootAbstractComp;
            set => PolyRootAbstractComp = (Poly_RootAbstract_Comp)value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();


        public Guid? TopLevelId { get; set; }
    }
}
