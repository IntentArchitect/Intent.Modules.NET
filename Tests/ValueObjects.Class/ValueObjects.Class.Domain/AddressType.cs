using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace ValueObjects.Class.Domain
{
    public enum AddressType
    {
        Residential,
        Office
    }
}