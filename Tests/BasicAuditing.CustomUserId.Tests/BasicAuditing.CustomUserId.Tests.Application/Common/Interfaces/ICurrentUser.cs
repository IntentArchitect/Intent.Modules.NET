using System;
using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserInterface", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid? Id { get; }
        string? Name { get; }
        ClaimsPrincipal Principal { get; }
    }
}