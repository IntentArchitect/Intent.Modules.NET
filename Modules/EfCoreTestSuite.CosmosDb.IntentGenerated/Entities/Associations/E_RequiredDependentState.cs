using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class E_RequiredDependent : IE_RequiredDependent
    {
        public Guid Id { get; set; }

        public string RequiredDependentAttr { get; set; }

        public virtual E_RequiredCompositeNav ERequiredCompositeNav { get; set; }

        IE_RequiredCompositeNav IE_RequiredDependent.ERequiredCompositeNav
        {
            get => ERequiredCompositeNav;
            set => ERequiredCompositeNav = (E_RequiredCompositeNav)value;
        }
    }
}