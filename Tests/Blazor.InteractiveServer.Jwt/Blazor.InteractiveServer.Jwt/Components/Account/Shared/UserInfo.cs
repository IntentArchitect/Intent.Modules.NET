using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.UserInfoTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Components.Account.Shared
{
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? RefreshUrl { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
    }
}