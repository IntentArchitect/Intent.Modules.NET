using System.Diagnostics;
using System.Security.Claims;
using Blazor.InteractiveAuto.AspNetCoreIdentity.Client.Components.Account.Shared;
using Blazor.InteractiveAuto.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.PersistingRevalidatingAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.AspNetCoreIdentity.Components.Account
{
    public class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IdentityOptions options;
        private readonly PersistingComponentStateSubscription subscription;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PersistentComponentState _persistentComponentState;
        private Task<AuthenticationState>? authenticationStateTask;

        public PersistingRevalidatingAuthenticationStateProvider(ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            PersistentComponentState persistentComponentState,
            IOptions<IdentityOptions> optionsAccessor) : base(loggerFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _persistentComponentState = persistentComponentState;
            options = optionsAccessor.Value;
            AuthenticationStateChanged += OnAuthenticationStateChanged;
            subscription = _persistentComponentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState,
            CancellationToken cancellationToken)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await ValidateSecurityStampAsync(userManager, authenticationState.User); ;
        }

        protected override void Dispose(bool disposing)
        {
            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
            base.Dispose(disposing);
        }

        private async Task<bool> ValidateSecurityStampAsync(
            UserManager<ApplicationUser> userManager,
            ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }
            var authenticationState = await authenticationStateTask;
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated == true)
            {
                var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
                var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
                var accessToken = principal.FindFirst("access_token")?.Value;

                if (userId != null && email != null)
                {
                    var userInfo = new UserInfo { UserId = userId, Email = email, AccessToken = accessToken };
                    _persistentComponentState.PersistAsJson(nameof(UserInfo), userInfo);
                }
            }
        }
    }
}