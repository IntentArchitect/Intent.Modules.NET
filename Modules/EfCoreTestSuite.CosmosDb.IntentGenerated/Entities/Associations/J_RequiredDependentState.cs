using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class J_RequiredDependent : IJ_RequiredDependent
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredDepAttr { get; set; }
    }
}