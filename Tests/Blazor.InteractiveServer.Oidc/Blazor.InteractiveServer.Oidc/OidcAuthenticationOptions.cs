using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.OidcAuthenticationOptionsTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc
{
    public class OidcAuthenticationOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DefaultScopes { get; set; }
    }
}