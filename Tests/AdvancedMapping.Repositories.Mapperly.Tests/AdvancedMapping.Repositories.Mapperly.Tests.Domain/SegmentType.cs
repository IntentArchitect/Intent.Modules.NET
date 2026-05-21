using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain
{
    public enum SegmentType
    {
        New = 1,
        Loyal = 2,
        AtRisk = 3
    }
}