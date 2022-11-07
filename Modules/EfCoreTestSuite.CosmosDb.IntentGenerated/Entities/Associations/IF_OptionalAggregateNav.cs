using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IF_OptionalAggregateNav
    {
        string PartitionKey { get; set; }

        string OptionalAggrNavAttr { get; set; }

        IF_OptionalDependent F_OptionalDependent { get; set; }
    }
}