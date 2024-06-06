using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Nullability
{
    public enum DefaultLiteralEnum
    {
        DefaultValue = 0,
        SecondaryValue = 1,
        TertiaryValue = 2
    }
}