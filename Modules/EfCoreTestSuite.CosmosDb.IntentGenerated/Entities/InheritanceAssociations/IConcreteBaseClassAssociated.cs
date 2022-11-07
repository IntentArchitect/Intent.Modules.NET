using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public interface IConcreteBaseClassAssociated
    {
        string PartitionKey { get; set; }

        string AssociatedField { get; set; }

        IConcreteBaseClass ConcreteBaseClass { get; set; }
    }
}