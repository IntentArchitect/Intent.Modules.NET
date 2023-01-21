using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IF_OptionalDependent
    {

        string PartitionKey { get; set; }

        string OptionalDependentAttr { get; set; }

        IF_OptionalAggregateNav FOptionalAggregateNav { get; set; }
    }
}