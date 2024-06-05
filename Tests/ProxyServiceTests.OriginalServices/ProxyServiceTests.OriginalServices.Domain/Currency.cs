using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Domain
{
    public enum Currency
    {
        ZAR,
        USD,
        EUR
    }
}