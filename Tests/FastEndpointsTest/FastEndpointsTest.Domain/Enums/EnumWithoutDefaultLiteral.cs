using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace FastEndpointsTest.Domain.Enums
{
    public enum EnumWithoutDefaultLiteral
    {
        FirstLiteral = 1,
        SecondLiteral = 2,
        ThirdLiteral = 3
    }
}