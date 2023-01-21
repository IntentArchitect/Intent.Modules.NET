using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class G_RequiredCompositeNav : IG_RequiredCompositeNav
    {

        public Guid Id { get; set; }

        public string ReqCompNavAttr { get; set; }

        public virtual ICollection<G_MultipleDependent> GMultipleDependents { get; set; } = new List<G_MultipleDependent>();

        ICollection<IG_MultipleDependent> IG_RequiredCompositeNav.GMultipleDependents
        {
            get => GMultipleDependents.CreateWrapper<IG_MultipleDependent, G_MultipleDependent>();
            set => GMultipleDependents = value.Cast<G_MultipleDependent>().ToList();
        }


    }
}
