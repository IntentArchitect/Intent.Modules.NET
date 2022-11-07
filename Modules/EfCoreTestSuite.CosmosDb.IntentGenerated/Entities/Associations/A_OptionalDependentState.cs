using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class A_OptionalDependent : IA_OptionalDependent
    {
        public Guid Id { get; set; }

        public string OptionalDependentAttr { get; set; }
    }
}