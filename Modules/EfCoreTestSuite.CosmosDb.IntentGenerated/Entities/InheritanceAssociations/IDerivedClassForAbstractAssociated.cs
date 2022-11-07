using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public interface IDerivedClassForAbstractAssociated
    {
        string PartitionKey { get; set; }

        string AssociatedField { get; set; }

        IDerivedClassForAbstract DerivedClassForAbstract { get; set; }
    }
}