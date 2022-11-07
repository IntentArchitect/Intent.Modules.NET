using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public interface IDerivedClassForAbstract : IAbstractBaseClass
    {
        string DerivedAttribute { get; set; }

        string PartitionKey { get; set; }
    }
}