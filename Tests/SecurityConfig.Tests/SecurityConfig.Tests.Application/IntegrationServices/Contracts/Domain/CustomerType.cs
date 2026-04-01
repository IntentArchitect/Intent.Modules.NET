using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.EnumContract", Version = "1.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices.Contracts.Domain
{
    public enum CustomerType
    {
        TypeA,
        TypeB
    }
}