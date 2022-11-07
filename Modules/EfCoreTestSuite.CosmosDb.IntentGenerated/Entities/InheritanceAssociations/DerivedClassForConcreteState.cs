using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public partial class DerivedClassForConcrete : ConcreteBaseClass, IDerivedClassForConcrete
    {
        public string PartitionKey { get; set; }

        public string DerivedAttribute { get; set; }
    }
}