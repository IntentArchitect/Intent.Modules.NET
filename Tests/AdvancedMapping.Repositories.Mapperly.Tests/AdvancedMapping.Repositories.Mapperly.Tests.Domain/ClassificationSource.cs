using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain
{
    public enum ClassificationSource
    {
        Manual = 1,
        MachineLearing = 2,
        RulesEngine = 3
    }
}