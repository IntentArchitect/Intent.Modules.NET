using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{
    public interface IBase
    {

        string BaseField1 { get; set; }

        string PartitionKey { get; set; }

        IBaseAssociated BaseAssociated { get; set; }
    }
}