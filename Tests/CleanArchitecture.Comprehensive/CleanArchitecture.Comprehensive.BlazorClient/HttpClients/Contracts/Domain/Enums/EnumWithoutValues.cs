using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.EnumContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Domain.Enums
{
    public enum EnumWithoutValues
    {
        LiteralOne,
        LiteralTwo,
        LiteralThree
    }
}