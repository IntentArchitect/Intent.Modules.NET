using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Nullability
{
    public enum MyEnum
    {
        First = 1,
        Second = 2
    }
}