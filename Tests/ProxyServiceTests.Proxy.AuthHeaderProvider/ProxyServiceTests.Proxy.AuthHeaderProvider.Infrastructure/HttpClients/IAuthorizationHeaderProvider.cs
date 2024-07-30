using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.AuthorizationHeaderProviderInterface", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.HttpClients
{
    public interface IAuthorizationHeaderProvider
    {
        string? GetAuthorizationHeader();
    }
}