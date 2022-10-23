using System;
using System.Collections.Generic;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract : IPoly_RootAbstract, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string AbstractField { get; set; }


        public Guid? Poly_RootAbstract_AggrId { get; set; }

        public virtual Poly_RootAbstract_Aggr Poly_RootAbstract_Aggr { get; set; }

        IPoly_RootAbstract_Aggr IPoly_RootAbstract.Poly_RootAbstract_Aggr
        {
            get => Poly_RootAbstract_Aggr;
            set => Poly_RootAbstract_Aggr = (Poly_RootAbstract_Aggr)value;
        }

        public virtual Poly_RootAbstract_Comp Poly_RootAbstract_Comp { get; set; }

        IPoly_RootAbstract_Comp IPoly_RootAbstract.Poly_RootAbstract_Comp
        {
            get => Poly_RootAbstract_Comp;
            set => Poly_RootAbstract_Comp = (Poly_RootAbstract_Comp)value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();


        public Guid? TopLevelId { get; set; }
    }
}
