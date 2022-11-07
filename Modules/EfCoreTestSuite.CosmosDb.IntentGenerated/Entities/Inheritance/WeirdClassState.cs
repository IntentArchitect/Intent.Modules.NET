using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{
    public partial class WeirdClass : Composite, IWeirdClass
    {
        public string PartitionKey { get; set; }

        public string WeirdField { get; set; }
    }
}