using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{
    public interface IPoly_SecondLevel
    {
        string SecondField { get; set; }

        string PartitionKey { get; set; }

        IPoly_BaseClassNonAbstract BaseClassNonAbstracts { get; set; }
    }
}