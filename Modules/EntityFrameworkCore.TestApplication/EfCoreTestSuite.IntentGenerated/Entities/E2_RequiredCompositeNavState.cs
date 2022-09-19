using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities
{

    public partial class E2_RequiredCompositeNav : IE2_RequiredCompositeNav
    {

        public Guid Id
        { get; set; }

        public string Attribute
        { get; set; }

        public virtual E2_RequiredDependent E2_RequiredDependent
        { get; set; }

        IE2_RequiredDependent IE2_RequiredCompositeNav.E2_RequiredDependent
        {
            get => E2_RequiredDependent;
            set => E2_RequiredDependent = (E2_RequiredDependent)value;
        }


    }
}
