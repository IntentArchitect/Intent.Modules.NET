using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{
    public interface IPoly_BaseClassNonAbstract
    {

        string BaseField { get; set; }

        string PartitionKey { get; set; }
    }
}