using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.EnumContract", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Domain
{
    public enum Currency
    {
        ZAR,
        USD,
        EUR
    }
}