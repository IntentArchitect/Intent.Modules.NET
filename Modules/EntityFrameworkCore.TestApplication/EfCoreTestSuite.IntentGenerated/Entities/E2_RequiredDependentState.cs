using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class E2_RequiredDependent : IE2_RequiredDependent
    {

        public string Attribute
        { get; set; }

        public virtual E2_RequiredCompositeNav E2_RequiredCompositeNav
        { get; set; }

        IE2_RequiredCompositeNav IE2_RequiredDependent.E2_RequiredCompositeNav
        {
            get => E2_RequiredCompositeNav;
            set => E2_RequiredCompositeNav = (E2_RequiredCompositeNav)value;
        }


    }
}
