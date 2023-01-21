using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class E_RequiredCompositeNav : IE_RequiredCompositeNav
    {

        public Guid Id { get; set; }

        public string RequiredCompNavAttr { get; set; }

        public virtual E_RequiredDependent ERequiredDependent { get; set; }

        IE_RequiredDependent IE_RequiredCompositeNav.ERequiredDependent
        {
            get => ERequiredDependent;
            set => ERequiredDependent = (E_RequiredDependent)value;
        }


    }
}
