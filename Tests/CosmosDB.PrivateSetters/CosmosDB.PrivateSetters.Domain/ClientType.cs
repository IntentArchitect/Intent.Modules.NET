using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain
{
    public enum ClientType
    {
        Individual,
        Business
    }
}