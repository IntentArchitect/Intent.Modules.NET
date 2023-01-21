using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class E2_RequiredCompositeNav : IE2_RequiredCompositeNav
    {

        public Guid Id { get; set; }

        public string ReqCompNavAttr { get; set; }

        public virtual E2_RequiredDependent E2RequiredDependent { get; set; }

        IE2_RequiredDependent IE2_RequiredCompositeNav.E2RequiredDependent
        {
            get => E2RequiredDependent;
            set => E2RequiredDependent = (E2_RequiredDependent)value;
        }


    }
}
