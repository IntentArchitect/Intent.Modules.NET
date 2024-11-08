using System;
using System.Collections.Generic;
using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.TokenServiceInterface", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Api.Services
{
    public interface ITokenService
    {
        (string Token, DateTime Expiry) GenerateAccessToken(string username, IList<Claim> claims);
        (string Token, DateTime Expiry) GenerateRefreshToken(string username);
        string? GetUsernameFromRefreshToken(string? token);
    }
}