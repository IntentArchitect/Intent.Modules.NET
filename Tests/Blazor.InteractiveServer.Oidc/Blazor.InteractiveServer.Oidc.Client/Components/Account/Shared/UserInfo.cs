using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.UserInfoTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Oidc.Client.Components.Account.Shared
{
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public string? AccessToken { get; set; }
    }
}