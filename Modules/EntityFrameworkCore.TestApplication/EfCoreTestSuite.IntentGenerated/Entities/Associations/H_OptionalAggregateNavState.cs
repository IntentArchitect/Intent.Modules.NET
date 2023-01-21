using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class H_OptionalAggregateNav : IH_OptionalAggregateNav
    {

        public Guid Id { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual ICollection<H_MultipleDependent> HMultipleDependents { get; set; } = new List<H_MultipleDependent>();

        ICollection<IH_MultipleDependent> IH_OptionalAggregateNav.HMultipleDependents
        {
            get => HMultipleDependents.CreateWrapper<IH_MultipleDependent, H_MultipleDependent>();
            set => HMultipleDependents = value.Cast<H_MultipleDependent>().ToList();
        }


    }
}
