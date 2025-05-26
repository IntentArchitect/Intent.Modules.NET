using System.Security.Claims;
using Duende.IdentityModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using OcelotTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace OcelotTest.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Subject)?.Value;
        public string? UserName => GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Name)?.Value;

        public async Task<bool> AuthorizeAsync(string policy)
        {
            if (GetClaimsPrincipal() == null)
            {
                return false;
            }

            var authService = GetAuthorizationService();

            if (authService == null)
            {
                return false;
            }

            var claimsPrinciple = GetClaimsPrincipal();
            return (await authService.AuthorizeAsync(claimsPrinciple!, policy))?.Succeeded ?? false;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            return await Task.FromResult(GetClaimsPrincipal()?.IsInRole(role) ?? false);
        }

        private ClaimsPrincipal? GetClaimsPrincipal() => _httpContextAccessor?.HttpContext?.User;

        private IAuthorizationService? GetAuthorizationService() => _httpContextAccessor?.HttpContext?.RequestServices.GetService(typeof(IAuthorizationService)) as IAuthorizationService;
    }
}