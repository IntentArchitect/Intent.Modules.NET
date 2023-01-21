using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class A_RequiredComposite : IA_RequiredComposite
    {
        public Guid Id { get; set; }

        public string RequiredCompositeAttr { get; set; }

        public string PartitionKey { get; set; }

        public virtual A_OptionalDependent AOptionalDependent { get; set; }

        IA_OptionalDependent IA_RequiredComposite.AOptionalDependent
        {
            get => AOptionalDependent;
            set => AOptionalDependent = (A_OptionalDependent)value;
        }
    }
}