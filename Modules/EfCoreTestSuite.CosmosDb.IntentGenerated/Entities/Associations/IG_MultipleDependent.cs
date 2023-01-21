using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public interface IG_MultipleDependent
    {

        string MultipleDepAttr { get; set; }

        IG_RequiredCompositeNav GRequiredCompositeNav { get; set; }
    }
}