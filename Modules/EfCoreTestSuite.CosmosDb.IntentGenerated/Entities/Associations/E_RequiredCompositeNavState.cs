using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class E_RequiredCompositeNav : IE_RequiredCompositeNav
    {
        public Guid Id { get; set; }

        public string RequiredCompositeNavAttr { get; set; }

        public string PartitionKey { get; set; }

        public virtual E_RequiredDependent ERequiredDependent { get; set; }

        IE_RequiredDependent IE_RequiredCompositeNav.ERequiredDependent
        {
            get => ERequiredDependent;
            set => ERequiredDependent = (E_RequiredDependent)value;
        }
    }
}