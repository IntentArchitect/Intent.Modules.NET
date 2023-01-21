using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class G_MultipleDependent : IG_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid GRequiredCompositeNavId { get; set; }

        public virtual G_RequiredCompositeNav GRequiredCompositeNav { get; set; }

        IG_RequiredCompositeNav IG_MultipleDependent.GRequiredCompositeNav
        {
            get => GRequiredCompositeNav;
            set => GRequiredCompositeNav = (G_RequiredCompositeNav)value;
        }


    }
}
