using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.TPH.IntentGenerated.DomainEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_SecondLevel : IPoly_SecondLevel, IHasDomainEvent
    {

        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public virtual ICollection<Poly_BaseClassNonAbstract> BaseClassNonAbstracts { get; set; } = new List<Poly_BaseClassNonAbstract>();

        ICollection<IPoly_BaseClassNonAbstract> IPoly_SecondLevel.BaseClassNonAbstracts
        {
            get => BaseClassNonAbstracts.CreateWrapper<IPoly_BaseClassNonAbstract, Poly_BaseClassNonAbstract>();
            set => BaseClassNonAbstracts = value.Cast<Poly_BaseClassNonAbstract>().ToList();
        }



        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
