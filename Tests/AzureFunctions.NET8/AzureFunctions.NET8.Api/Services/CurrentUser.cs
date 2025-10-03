using System.Security.Claims;
using AzureFunctions.NET8.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.Jwt.CurrentUser", Version = "1.0")]

namespace AzureFunctions.NET8.Api.Services
{
    public class CurrentUser : ICurrentUser
    {
        public CurrentUser(ClaimsPrincipal principal, string? accessToken)
        {
            Principal = principal;
            AccessToken = accessToken;
        }

        public ClaimsPrincipal Principal { get; }
        public string? AccessToken { get; }
        public string? Id => Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? Name => Principal.FindFirst(ClaimTypes.Name)?.Value;
    }
}