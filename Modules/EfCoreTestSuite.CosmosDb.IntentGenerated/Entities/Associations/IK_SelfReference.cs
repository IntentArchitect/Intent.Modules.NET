using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IK_SelfReference
    {

        string PartitionKey { get; set; }

        string SelfRefAttr { get; set; }

        IK_SelfReference KSelfReferenceAssociation { get; set; }
    }
}