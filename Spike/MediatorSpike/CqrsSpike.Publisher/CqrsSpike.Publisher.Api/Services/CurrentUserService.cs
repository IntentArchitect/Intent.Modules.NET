using System.Security.Claims;
using CqrsSpike.Publisher.Application.Common.Interfaces;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace CqrsSpike.Publisher.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ICurrentUser?> GetAsync()
        {
            var claimsPrincipal = GetClaimsPrincipal();

            // HttpContext.User is never null; an anonymous caller gets an unauthenticated principal.
            if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            {
                return Task.FromResult((ICurrentUser?)null);
            }

            ICurrentUser currentUser = new CurrentUser(
                    GetUserId(claimsPrincipal),
                    GetUserName(claimsPrincipal),
                    claimsPrincipal);

            return Task.FromResult<ICurrentUser?>(currentUser);
        }

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

        private static string? GetUserName(ClaimsPrincipal? claimsPrincipal) => claimsPrincipal?.FindFirst(JwtClaimTypes.Name)?.Value;

        private static string? GetUserId(ClaimsPrincipal? claimsPrincipal) => claimsPrincipal?.FindFirst(JwtClaimTypes.Subject)?.Value;
    }

    public record CurrentUser(string? Id, string? Name, ClaimsPrincipal Principal) : ICurrentUser;

}