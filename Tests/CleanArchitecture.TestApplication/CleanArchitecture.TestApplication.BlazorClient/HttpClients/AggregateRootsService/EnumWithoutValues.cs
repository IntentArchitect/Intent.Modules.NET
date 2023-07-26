using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.EnumContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public enum EnumWithoutValues
    {
        LiteralOne,
        LiteralTwo,
        LiteralThree
    }
}