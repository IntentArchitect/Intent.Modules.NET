using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using Blazor.InteractiveWebAssembly.Oidc.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.PersistingServerAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Oidc.Components.Account
{
    public class PersistingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
    {
        private readonly IdentityOptions options;
        private readonly PersistingComponentStateSubscription subscription;
        private readonly PersistentComponentState _persistentComponentState;
        private Task<AuthenticationState>? authenticationStateTask;

        public PersistingServerAuthenticationStateProvider(PersistentComponentState persistentComponentState,
            IOptions<IdentityOptions> optionsAccessor)
        {
            _persistentComponentState = persistentComponentState;
            options = optionsAccessor.Value;
            AuthenticationStateChanged += OnAuthenticationStateChanged;
            subscription = _persistentComponentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        public void Dispose()
        {
            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                return;
            }
            var authenticationState = await authenticationStateTask;
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated == true)
            {
                var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
                var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
                var accessToken = principal.FindFirst("access_token")?.Value;
                var expiresAtClaim = principal.FindFirst("expires_at")?.Value;

                if (!DateTime.TryParse(expiresAtClaim, null, DateTimeStyles.RoundtripKind, out var expiresAt))
                {
                    expiresAt = DateTime.UtcNow.AddHours(1);
                }

                if (userId != null && email != null)
                {
                    var userInfo = new UserInfo { UserId = userId, Email = email, AccessToken = accessToken, AccessTokenExpiresAt = expiresAt };
                    _persistentComponentState.PersistAsJson(nameof(UserInfo), userInfo);
                }
            }
        }
    }
}