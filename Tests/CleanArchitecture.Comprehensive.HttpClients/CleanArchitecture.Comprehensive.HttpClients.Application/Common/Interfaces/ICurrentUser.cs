using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        string? Id { get; }
        string? Name { get; }
        ClaimsPrincipal Principal { get; }
    }
}