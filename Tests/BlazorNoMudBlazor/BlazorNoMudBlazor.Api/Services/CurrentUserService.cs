using System.Security.Claims;
using BlazorNoMudBlazor.Application.Common.Interfaces;
using BlazorNoMudBlazor.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Services
{
    public class CurrentUserService : ICurrentUserService, ISetUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IAuthorizationService _authorizationService;
        private ClaimsPrincipal? _cachedUser;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authStateProvider,
            IAuthorizationService authorizationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
            _authorizationService = authorizationService;
        }

        public async Task<ICurrentUser?> GetAsync()
        {
            var user = await GetPrincipalAsync();

            if (user is null)
            {
                return null;
            }
            return new CurrentUser(GetUserId(user), GetUserName(user), user);
        }

        public async Task<bool> AuthorizeAsync(string policy)
        {
            var user = await GetPrincipalAsync();

            if (user is null)
            {
                return false;
            }
            var result = await _authorizationService.AuthorizeAsync(user, policy);
            return result.Succeeded;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var user = await GetPrincipalAsync();
            return user?.IsInRole(role) ?? false;
        }

        void ISetUserContext.SetContext(ClaimsPrincipal principal) => _cachedUser = principal;

        private async Task<ClaimsPrincipal?> GetPrincipalAsync()
        {
            if (_cachedUser is not null)
            {
                return _cachedUser;
            }

            var httpUser = _httpContextAccessor.HttpContext?.User;

            if (httpUser?.Identity?.IsAuthenticated == true)
            {
                _cachedUser = httpUser;
            }
            else
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                _cachedUser = authState.User;
            }

            return _cachedUser;
        }

        private static string? GetUserName(ClaimsPrincipal user) => user.Identity?.Name;

        private static string? GetUserId(ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public record CurrentUser(string? Id, string? Name, ClaimsPrincipal Principal) : ICurrentUser;

}