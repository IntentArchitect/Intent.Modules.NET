using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class E_RequiredDependent : IE_RequiredDependent
    {

        public Guid Id { get; set; }

        public string RequiredDepAttr { get; set; }

        public virtual E_RequiredCompositeNav E_RequiredCompositeNav { get; set; }

        IE_RequiredCompositeNav IE_RequiredDependent.E_RequiredCompositeNav
        {
            get => E_RequiredCompositeNav;
            set => E_RequiredCompositeNav = (E_RequiredCompositeNav)value;
        }


    }
}
