using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class D_MultipleDependent : ID_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDependentAttr { get; set; }

        public string PartitionKey { get; set; }

        public Guid? DOptionalAggregateId { get; set; }
    }
}