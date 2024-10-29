using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AzureFunctions.NET6.Domain
{
    public enum DomainNotificationType
    {
        TypeA = 1,
        TypeB = 2
    }
}