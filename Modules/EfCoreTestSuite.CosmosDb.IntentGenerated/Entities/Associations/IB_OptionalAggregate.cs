using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IB_OptionalAggregate
    {

        string OptionalAggregateAttr { get; set; }

        string PartitionKey { get; set; }

        IB_OptionalDependent B_OptionalDependent { get; set; }
    }
}