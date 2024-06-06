using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Nullability
{
    public enum NoDefaultLiteralEnum
    {
        First = 1,
        Second = 2
    }
}