using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IH_MultipleDependent
    {

        string PartitionKey { get; set; }

        string MultipleDepAttr { get; set; }

        IH_OptionalAggregateNav HOptionalAggregateNav { get; set; }
    }
}