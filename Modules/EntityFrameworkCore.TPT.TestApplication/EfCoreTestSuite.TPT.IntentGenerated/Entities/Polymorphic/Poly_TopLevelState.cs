using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.TPT.IntentGenerated.Core;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_TopLevel : IPoly_TopLevel, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string TopField { get; set; }

        public virtual ICollection<Poly_RootAbstract> RootAbstracts { get; set; } = new List<Poly_RootAbstract>();

        ICollection<IPoly_RootAbstract> IPoly_TopLevel.RootAbstracts
        {
            get => RootAbstracts.CreateWrapper<IPoly_RootAbstract, Poly_RootAbstract>();
            set => RootAbstracts = value.Cast<Poly_RootAbstract>().ToList();
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
