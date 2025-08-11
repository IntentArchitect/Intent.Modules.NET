using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.SetUserContextInterface", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Services
{
    public interface ISetUserContext
    {
        void SetContext(ClaimsPrincipal principal);
    }
}