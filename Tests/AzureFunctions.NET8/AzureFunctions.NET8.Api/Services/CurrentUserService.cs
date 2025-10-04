using AzureFunctions.NET8.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserService", Version = "1.0")]

namespace AzureFunctions.NET8.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId { get; set; }
        public string? UserName { get; set; }

        public Task<ICurrentUser?> GetAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Task.FromResult<ICurrentUser?>(null);
            }

            var principal = httpContext.User;
            if (principal?.Identity?.IsAuthenticated == true)
            {
                var rawToken = ResolveToken(httpContext);
                return Task.FromResult<ICurrentUser?>(new CurrentUser(principal, rawToken));
            }

            return Task.FromResult<ICurrentUser?>(null);
        }

        public async Task<bool> AuthorizeAsync(string policy)
        {
            var currentUser = await GetAsync();
            if (currentUser == null)
                return false;

            // Basic authorization logic - can be extended based on your policy requirements
            // For now, any authenticated user is authorized
            return true;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var currentUser = await GetAsync();
            if (currentUser == null)
                return false;

            return currentUser.Principal.IsInRole(role);
        }

        private static string? ResolveToken(HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue("RawToken", out var tokenObject) && tokenObject is string rawToken)
            {
                return rawToken;
            }

            return null;
        }
    }
}