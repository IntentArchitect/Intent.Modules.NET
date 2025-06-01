using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Domain
{
    public enum OrgType
    {
        Corp,
        PublicCo,
        PrivateCo
    }
}