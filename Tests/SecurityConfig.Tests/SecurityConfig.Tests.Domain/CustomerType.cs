using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace SecurityConfig.Tests.Domain
{
    public enum CustomerType
    {
        TypeA,
        TypeB
    }
}