using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public interface IDerivedClassForConcreteAssociated
    {
        string PartitionKey { get; set; }

        string Associatedfield { get; set; }

        IDerivedClassForConcrete DerivedClassForConcrete { get; set; }
    }
}