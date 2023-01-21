using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IE_RequiredCompositeNav
    {

        string RequiredCompositeNavAttr { get; set; }

        string PartitionKey { get; set; }

        IE_RequiredDependent ERequiredDependent { get; set; }
    }
}