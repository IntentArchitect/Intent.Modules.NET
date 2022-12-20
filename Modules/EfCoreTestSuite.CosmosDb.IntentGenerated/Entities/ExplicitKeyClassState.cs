using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities
{
    public partial class ExplicitKeyClass : IExplicitKeyClass
    {
        public Guid Id { get; set; }

        public string Attribute { get; set; }
    }
}