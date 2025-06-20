using System.Security.Claims;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Interfaces;
using Duende.IdentityModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace CosmosDB.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [IntentIgnore]
        public string? UserId => GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Subject)?.Value ?? "Unknown";
        [IntentIgnore]
        public string? UserName => GetClaimsPrincipal()?.FindFirst(JwtClaimTypes.Name)?.Value ?? "Unknown";

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