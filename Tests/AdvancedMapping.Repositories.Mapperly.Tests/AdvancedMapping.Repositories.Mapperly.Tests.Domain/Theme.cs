using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain
{
    public enum Theme
    {
        Dark = 1,
        Light = 2
    }
}