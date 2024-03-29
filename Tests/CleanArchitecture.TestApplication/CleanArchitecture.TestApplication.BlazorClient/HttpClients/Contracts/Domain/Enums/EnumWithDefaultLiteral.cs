using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.EnumContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Domain.Enums
{
    public enum EnumWithDefaultLiteral
    {
        DefaultLiteral = 0,
        FirstLiteral = 1,
        SecondLiteral = 2,
        ThirdLiteral = 3
    }
}